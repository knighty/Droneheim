using Droneheim.GUI.Properties.Editors;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.UI;
using static Droneheim.GUI.Stylesheet;

namespace Droneheim.GUI
{
	internal class StyleProperty
	{
		internal object value;

		internal T GetValue<T>()
		{
			return (T)value;
		}
	}

	enum StyleUnit
	{
		Absolute,
		EM
	}

	enum StyleProperties
	{
		// Font
		FontSize,
		FontWeight,
		Font,
		Color,

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

	class StyleRule
	{
		internal string[] path;

		public StyleRule(string path)
		{
			this.path = path.Split(' ');
		}

		public StyleRule(List<string> path)
		{
			this.path = path.ToArray();
		}

		public int Matches(StyleRule b)
		{
			int j = 0;
			int i = 0;
			for (; i < path.Length; i++)
			{
				bool found = false;
				for (; j < b.path.Length; j++)
				{
					if (b.path[j] == path[i])
					{
						found = true;
						break;
					}
				}
				if (!found)
					return 0;
			}

			if (j == b.path.Length - 1) return 2;
			return 1;
		}
	}

	// Rule: Body .Window .Title
	// Element: Body Element Panel .Window Panel .Title

	class StyleTreeNode
	{
		internal Dictionary<string, StyleTreeNode> children = new Dictionary<string, StyleTreeNode>();
		internal List<StyleClass> classes = new List<StyleClass>();
		internal string name;

		public void AddStyleClass(StyleRule rule, StyleClass styleClass, int level)
		{
			if (level > rule.path.Length - 1)
			{
				classes.Add(styleClass);
				return;
			}

			string pathSegment = rule.path[level];
			StyleTreeNode node;
			if (!children.TryGetValue(pathSegment, out node))
			{
				node = children[pathSegment] = new StyleTreeNode();
			}
			node.AddStyleClass(rule, styleClass, level + 1);
		}

		public void FindMatchingClasses(StyleRule rule, int level, ref List<StyleClass> matchedClasses, ref List<StyleClass> directClasses)
		{
			string pathSegment = rule.path[level];
			List<StyleClass> addTo = (level == rule.path.Length - 1) ? directClasses : matchedClasses;
			addTo.AddRange(classes);

			for (int j = level + 1; j < rule.path.Length; j++)
			{
				string s = rule.path[j];
				StyleTreeNode node;
				if (children.TryGetValue(s, out node))
				{
					node.FindMatchingClasses(rule, j, ref matchedClasses, ref directClasses);
				}
			}
		}

		public void Print(int level)
		{
			foreach(var kv in children)
			{
				string str = "";
				for(int i = 0; i < level; i++)
				{
					str += "    ";
				}
				Debug.Log(str + kv.Key + " (" + kv.Value.classes.Count + " classes)");
				kv.Value.Print(level+1);
			}
		}
	}

	class StyleTree : StyleTreeNode
	{

	}

	class StyleClass
	{
		internal Dictionary<StyleProperties, StyleProperty> properties = new Dictionary<StyleProperties, StyleProperty>();
		public StyleClass()
		{
		}

		public void Set<T>(StyleProperties property, T value)
		{
			properties[property] = new StyleProperty() { value = value };
		}

		public T Get<T>(StyleProperties name)
		{
			return (T)(properties[name].value);
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

	class Stylesheet : MonoBehaviour
	{
		internal StyleTree styleTree = new StyleTree();

		//protected Dictionary<StyleRule, StyleClass> styleClasses = new Dictionary<StyleRule, StyleClass>();

		protected Dictionary<int, ComputedStyle> computedStyles = new Dictionary<int, ComputedStyle>();

		internal Dictionary<StyleProperties, StyleDelegate> styleDelegates = new Dictionary<StyleProperties, StyleDelegate>();

		public delegate void StyleDelegate(StyleProperty property, StyledElement element);

		public Stylesheet()
		{
			styleDelegates[StyleProperties.Font] = (StyleProperty property, StyledElement element) =>
			{
				if (element.TextComponent != null)
				{
					element.TextComponent.font = property.GetValue<Font>();
				}

			};
			styleDelegates[StyleProperties.FontSize] = (StyleProperty property, StyledElement element) =>
			{
				if (element.TextComponent != null)
				{
					element.TextComponent.fontSize = property.GetValue<int>();
				}
			};

			styleDelegates[StyleProperties.FontWeight] = (StyleProperty property, StyledElement element) =>
			{
				if (element.TextComponent != null)
				{
					element.TextComponent.fontStyle = property.GetValue<FontStyle>();
				}
			};

			styleDelegates[StyleProperties.Color] = (StyleProperty property, StyledElement element) =>
			{
				if (element.TextComponent != null)
				{
					element.TextComponent.color = property.GetValue<Color>();
				}
			};

			styleDelegates[StyleProperties.Padding] = (StyleProperty property, StyledElement element) =>
			{
				if (element.LayoutGroupComponent != null)
				{
					element.LayoutGroupComponent.padding = property.GetValue<RectOffset>();
				}
			};

			styleDelegates[StyleProperties.Width] = (StyleProperty property, StyledElement element) =>
			{
				if (element.RectTransformComponent != null)
				{
					element.RectTransformComponent.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, property.GetValue<int>());
				}
			};

			styleDelegates[StyleProperties.Height] = (StyleProperty property, StyledElement element) =>
			{
				if (element.RectTransformComponent != null)
				{
					element.RectTransformComponent.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, property.GetValue<int>());
				}
			};

			styleDelegates[StyleProperties.BackgroundImage] = (StyleProperty property, StyledElement element) =>
			{
				if (element.ImageComponent != null)
				{
					element.ImageComponent.sprite = property.GetValue<Sprite>();
					element.ImageComponent.type = Image.Type.Sliced;
				}
			};

			styleDelegates[StyleProperties.BackgroundColor] = (StyleProperty property, StyledElement element) =>
			{
				if (element.ImageComponent != null)
				{
					element.ImageComponent.color = property.GetValue<Color>();
				}
			};
		}

		public void StyleElement(StyledElement element, ComputedStyle computed)
		{
			foreach (KeyValuePair<StyleProperties, StyleProperty> kv in computed.properties)
			{
				styleDelegates[kv.Key](kv.Value, element);
			}
		}

		static bool IsInherited(StyleProperties property)
		{
			switch (property)
			{
				case StyleProperties.FontSize:
				case StyleProperties.FontWeight:
				case StyleProperties.Font:
				case StyleProperties.Color:
					return true;
			}
			return false;
		}

		public void AddStyle(string rule, StyleClass style)
		{
			styleTree.AddStyleClass(new StyleRule(rule), style, 0);
			//styleClasses.Add(new StyleRule(rule), style);
		}

		public void AddStyle(string rule, StyleClass.Factory f)
		{
			StyleClass style = StyleClass.Create(f);
			styleTree.AddStyleClass(new StyleRule(rule), style, 0);
			//styleClasses.Add(new StyleRule(rule), style);
		}

		public ComputedStyle GetComputedStyle(StyleRule rule)
		{
			string hashStr = rule.path.Aggregate("", (acc, x) => acc + x + " ");
			int hash = hashStr.GetHashCode();

			ComputedStyle computed;
			if (!computedStyles.TryGetValue(hash, out computed))
			{
				computed = new ComputedStyle();

				List<StyleClass> matchingStyleClasses = new List<StyleClass>(), directStyleClasses = new List<StyleClass>();
				styleTree.FindMatchingClasses(rule, 0, ref matchingStyleClasses, ref directStyleClasses);

				Debug.Log(hashStr + " found " + matchingStyleClasses.Count + " matching classes, " + directStyleClasses.Count + " direct classes");

				foreach (var styleClass in matchingStyleClasses)
				{
					foreach (KeyValuePair<StyleProperties, StyleProperty> property in styleClass.properties)
					{
						if (IsInherited(property.Key))
						{
							computed.properties[property.Key] = property.Value;
						}
					}
				}

				foreach (var styleClass in directStyleClasses)
				{
					foreach (KeyValuePair<StyleProperties, StyleProperty> property in styleClass.properties)
					{
						computed.properties[property.Key] = property.Value;
					}
				}

				/*foreach (KeyValuePair<StyleRule, StyleClass> kv in styleClasses)
				{
					int match = kv.Key.Matches(rule);
					Debug.Log(match);
					if (match > 0)
					{
						foreach (KeyValuePair<StyleProperties, StyleProperty> property in kv.Value.properties)
						{
							if (IsInherited(property.Key) || match == 2)
							{
								computed.properties[property.Key] = property.Value;
							}
						}
					}
				}*/
			}

			return computed;
		}
	}
}
