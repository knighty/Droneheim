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
		private StyleElementState state = StyleElementState.None;
		public StyleElementState State
		{
			get => state;
			set
			{
				state = value;
				computed = null;
				styleRule = null;
				Style();
			}
		}

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
			get => styleRule ??= (ElementType == null ? ParentNode.StyleRule : new StyleRuleSegment(ElementType, Classes, State));
		}

		ComputedStyle computed = null;
		public ComputedStyle ComputedStyle
		{
			get => computed ??= Stylesheet.GetComputedStyle(this);
		}

		protected Stylesheet stylesheet = null;
		public Stylesheet Stylesheet
		{
			get => stylesheet ??= gameObject.GetComponentInParent<Stylesheet>();
		}

		protected bool componentFlagsCalculated = false;
		protected StyleComponentFlag componentFlags;
		public StyleComponentFlag ComponentFlags
		{
			get
			{
				if (!componentFlagsCalculated)
				{
					StyleComponentFlag flag = StyleComponentFlag.None;
					flag |= TextComponent != null ? StyleComponentFlag.Text : StyleComponentFlag.None;
					flag |= ImageComponent != null ? StyleComponentFlag.Image : StyleComponentFlag.None;
					flag |= RectTransformComponent != null ? StyleComponentFlag.RectTransform : StyleComponentFlag.None;
					flag |= LayoutGroupComponent != null ? StyleComponentFlag.Layout : StyleComponentFlag.None;
					componentFlags = flag;
				}

				return componentFlags;
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
