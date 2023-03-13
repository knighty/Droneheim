using Droneheim.GUI.Properties;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Droneheim.GUI
{
	class StyledElement : MonoBehaviour
	{
		public Text TextComponent;
		public Image ImageComponent;
		public RectTransform RectTransformComponent;
		public LayoutGroup LayoutGroupComponent;

		public string ElementType { get; set; } = null;
		public List<string> Classes { get; set; } = new List<string>();

		public void SetTypeClasses(string elementType, string classes = "")
		{
			ElementType = elementType;
			Classes = classes.Split(',').ToList();
		}

		public StyledElement ParentNode
		{
			get
			{
				return gameObject.transform.parent.GetComponentInParent<StyledElement>();
			}
		}

		protected StyleRuleSegment styleRule = null;
		public StyleRuleSegment StyleRule
		{
			get
			{
				if (styleRule == null)
				{
					if (ElementType == null)
					{
						styleRule = ParentNode.StyleRule;
					}
					else
					{
						styleRule = new StyleRuleSegment(ElementType, Classes);
					}
				}
				return styleRule;
			}
		}

		ComputedStyle computed = null;
		public ComputedStyle ComputedStyle
		{
			get
			{
				if (computed == null)
				{
					computed = Stylesheet.GetComputedStyle(this);
				}
				return computed;
			}
		}

		protected Stylesheet stylesheet = null;
		public Stylesheet Stylesheet
		{
			get => stylesheet = stylesheet ?? gameObject.GetComponentInParent<Stylesheet>();
		}

		public StyleComponentFlag ComponentFlags
		{
			get
			{
				StyleComponentFlag flag = StyleComponentFlag.None;
				flag |= TextComponent != null ? StyleComponentFlag.Text : StyleComponentFlag.None;
				flag |= ImageComponent != null ? StyleComponentFlag.Image : StyleComponentFlag.None;
				flag |= RectTransformComponent != null ? StyleComponentFlag.RectTransform : StyleComponentFlag.None;
				flag |= LayoutGroupComponent != null ? StyleComponentFlag.Layout : StyleComponentFlag.None;

				return flag;
			}
		}

		public void Awake()
		{

		}

		public void Style()
		{
			TextComponent = GetComponent<Text>();
			ImageComponent = GetComponent<Image>();
			RectTransformComponent = GetComponent<RectTransform>();
			LayoutGroupComponent = GetComponent<LayoutGroup>();

			Stylesheet stylesheet = Stylesheet;
			stylesheet.StyleElement(this, ComputedStyle);
		}

		public void Start()
		{
			Style();
		}
	}
}
