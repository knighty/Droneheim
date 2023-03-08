using Droneheim.GUI.Properties.Editors;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Droneheim.GUI
{
	internal class StyleProperty
	{
		internal object value;
		internal bool inherited;

		public bool Inherited { get { return inherited; } }

		internal T GetValue<T>()
		{
			return (T)value;
		}
	}

	enum StyleUnit
	{
		Inherit,
		Absolute,
		Percentage,
		EM
	}

	class StyleDimension
	{
		StyleUnit unit;
		float value;
	}

	enum StyleProperties
	{
		// Font
		FontSize,
		FontStyle,
		Font,
		Color,
		TextAlign,

		// Background
		BackgroundImage,
		BackgroundColor,

		// Layout
		Padding,
		Margin,
		Width,
		Height,
		Direction
	}

	enum StyleElementState
	{
		None,
		Hover,
		Active
	}

	class StyleRuleSegment
	{
		internal string ElementType;
		private HashSet<string> ClassesHashSet = new HashSet<string>();

		private List<string> classes = new List<string>();
		internal List<string> Classes
		{
			set
			{
				foreach (var c in value)
				{
					ClassesHashSet.Add(c);
				}
				classes = value;
			}
			get
			{
				return classes;
			}
		}
		internal string State;

		public StyleRuleSegment(string str)
		{
			var segments = str.Split('.');
			ElementType = segments[0];
			Classes = segments.Skip(1).ToList();
		}

		public StyleRuleSegment(string type, List<string> classes)
		{
			ElementType = type;
			Classes = classes;
		}

		public bool Match(StyleRuleSegment other)
		{
			if (other.ElementType != ElementType && ElementType != null && ElementType != "") return false;
			if (other.State != State) return false;
			foreach (var str in Classes)
			{
				if (!other.ClassesHashSet.Contains(str)) return false;
			}
			return true;
		}

		public override string ToString()
		{
			string s = ElementType;
			foreach (var str in Classes) { s += "." + str; }
			return s;
		}
	}

	class StyleRule
	{
		internal StyleClass style;
		internal List<StyleRuleSegment> segments;

		public StyleRule(string path, StyleClass style)
		{
			this.segments = path.Split(' ').Select(str => new StyleRuleSegment(str)).ToList();
			this.style = style;
		}

		public bool Matches(StyledElement element)
		{
			if (!segments[segments.Count - 1].Match(element.StyleRule))
				return false;

			int i = segments.Count - 2;
			element = element.ParentNode;
			while (i >= 0)
			{
				while (element != null)
				{
					if (segments[i].Match(element.StyleRule))
					{
						break;
					}
					element = element.ParentNode;
				}
				i--;
				if (element == null)
					return false;
			}

			return i <= 0;
		}

		public override string ToString()
		{
			string s = "";
			foreach (var segment in segments) { s += segment.ToString(); }
			return s;
		}
	}

	class StyleClass
	{
		internal Dictionary<StyleProperties, StyleProperty> properties = new Dictionary<StyleProperties, StyleProperty>();
		public StyleClass()
		{
			SetInherited(StyleProperties.FontSize);
			SetInherited(StyleProperties.Font);
			SetInherited(StyleProperties.FontStyle);
			SetInherited(StyleProperties.Color);
		}

		public void Set<T>(StyleProperties property, T value)
		{
			properties[property] = new StyleProperty() { value = value };
		}

		public void SetInherited(StyleProperties property)
		{
			properties[property] = new StyleProperty() { inherited = true };
		}

		public T Get<T>(StyleProperties name)
		{
			return (T)(properties[name].value);
		}

		public StyleProperty GetProperty(StyleProperties name)
		{
			StyleProperty p = null;
			properties.TryGetValue(name, out p);
			return p;
		}

		public delegate void Factory(StyleClass style);

		public static StyleClass Create(Factory f)
		{
			StyleClass s = new StyleClass();
			f(s);
			return s;
		}
	}

	class ComputedStyle : StyleClass
	{

	}

	[Flags]
	enum StyleComponentFlag
	{
		None = 0,
		Text = 1 << 0,
		Layout = 1 << 1,
		Image = 1 << 2,
		RectTransform = 1 << 3
	}

	class Stylesheet : MonoBehaviour
	{
		protected List<StyleRule> styleRules = new List<StyleRule>();
		protected Dictionary<string, StyleRule> tagRules = new Dictionary<string, StyleRule>();
		protected Dictionary<string, StyleRule> classRules = new Dictionary<string, StyleRule>();

		//protected Dictionary<StyleRule, StyleClass> styleClasses = new Dictionary<StyleRule, StyleClass>();

		protected Dictionary<int, ComputedStyle> computedStyles = new Dictionary<int, ComputedStyle>();

		internal Dictionary<StyleProperties, StyleDelegate> styleDelegates = new Dictionary<StyleProperties, StyleDelegate>();
		internal Dictionary<StyleProperties, StyleComponentFlag> styleComponentFlags = new Dictionary<StyleProperties, StyleComponentFlag>();

		public delegate void StyleDelegate(StyledElement element);

		public Stylesheet()
		{
			void DeclareDelegate<T>(StyleProperties prop, StyleComponentFlag flags, Action<StyledElement, T> setter)
			{
				styleDelegates[prop] = (StyledElement element) =>
				{
					Debug.Log($"Styling {element.StyleRule} with {prop}");
					StyledElement node = element;
					while (node != null)
					{
						StyleProperty p = node.ComputedStyle.GetProperty(prop);
						if (p != null && !p.inherited)
						{
							setter(element, p.GetValue<T>());
							break;
						}
						node = node.ParentNode;
					}
				};
				styleComponentFlags[prop] = flags;
			}

			DeclareDelegate(StyleProperties.Font, StyleComponentFlag.Text, (StyledElement element, Font value) => element.TextComponent.font = value);
			DeclareDelegate(StyleProperties.FontSize, StyleComponentFlag.Text, (StyledElement element, int value) => element.TextComponent.fontSize = value);
			DeclareDelegate(StyleProperties.FontStyle, StyleComponentFlag.Text, (StyledElement element, FontStyle value) => element.TextComponent.fontStyle = value);
			DeclareDelegate(StyleProperties.TextAlign, StyleComponentFlag.Text, (StyledElement element, TextAnchor value) => element.TextComponent.alignment = value);

			DeclareDelegate(StyleProperties.Color, StyleComponentFlag.Text, (StyledElement element, Color value) => element.TextComponent.color = value);

			DeclareDelegate(StyleProperties.Padding, StyleComponentFlag.Layout, (StyledElement element, RectOffset value) => element.LayoutGroupComponent.padding = value);

			DeclareDelegate(StyleProperties.Width, StyleComponentFlag.RectTransform, (StyledElement element, int value) => element.RectTransformComponent.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, value));
			DeclareDelegate(StyleProperties.Height, StyleComponentFlag.RectTransform, (StyledElement element, int value) => element.RectTransformComponent.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, value));

			DeclareDelegate(StyleProperties.BackgroundImage, StyleComponentFlag.Image, (StyledElement element, Sprite value) =>
			{
				element.ImageComponent.sprite = value;
				element.ImageComponent.type = Image.Type.Sliced;
			});
			DeclareDelegate(StyleProperties.BackgroundColor, StyleComponentFlag.Image, (StyledElement element, Color value) => element.ImageComponent.color = value);
		}

		public void StyleElement(StyledElement element, ComputedStyle computed)
		{
			foreach (var kv in computed.properties)
			{
				StyleComponentFlag required = styleComponentFlags[kv.Key];
				if ((element.ComponentFlags & required) == required)
				{
					styleDelegates[kv.Key](element);
				}
			}
		}

		public void AddStyle(string rule, StyleClass style)
		{
			styleRules.Add(new StyleRule(rule, style));
			//styleClasses.Add(new StyleRule(rule), style);
		}

		public void AddStyle(string rule, StyleClass.Factory f)
		{
			StyleClass style = StyleClass.Create(f);
			styleRules.Add(new StyleRule(rule, style));
		}

		public ComputedStyle GetComputedStyle(StyledElement element)
		{
			Debug.Log($"Computing {element.StyleRule}");
			ComputedStyle computed = new ComputedStyle();
			foreach (StyleRule rule in styleRules)
			{
				if (rule.Matches(element))
				{
					Debug.Log($"Matched {rule}");
					foreach (var property in rule.style.properties)
					{
						computed.properties[property.Key] = property.Value;
					}
				}
			}

			return computed;
		}
	}
}
