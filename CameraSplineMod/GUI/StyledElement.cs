using Droneheim.GUI.Properties;
using System.Collections.Generic;
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

		public string ElementType { get; set; } = "Element";
		public string StyleClass { get; set; } = null;

		protected Stylesheet stylesheet = null;
		public Stylesheet Stylesheet
		{
			get => stylesheet = stylesheet ?? gameObject.GetComponentInParent<Stylesheet>();
		}

		public StyleRule GetStyleRule()
		{
			List<string> pathElements = new List<string>();
			StyledElement comp = this;
			while (comp != null)
			{
				string str = "";
				/*if (comp.StyleClass != null)
				{
					str += "." + comp.StyleClass;
					pathElements.Add("." + comp.StyleClass);
				}
				if (comp.ElementType != null)
				{
					pathElements.Add(comp.ElementType);
				}*/
				if (comp.ElementType != null)
				{
					str += comp.ElementType;
				}
				if (comp.StyleClass != null)
				{
					str += "." + comp.StyleClass;
				}
				pathElements.Add(str);
				comp = comp.gameObject.transform.parent.GetComponentInParent<StyledElement>();
			}

			pathElements.Reverse();

			return new StyleRule(pathElements);
		}

		public void Style()
		{
			Stylesheet stylesheet = Stylesheet;

			TextComponent = GetComponent<Text>();
			ImageComponent = GetComponent<Image>();
			RectTransformComponent = GetComponent<RectTransform>();
			LayoutGroupComponent = GetComponent<LayoutGroup>();

			ComputedStyle computed = stylesheet.GetComputedStyle(GetStyleRule());
			stylesheet.StyleElement(this, computed);
		}

		public void Start()
		{
			Style();
		}
	}
}
