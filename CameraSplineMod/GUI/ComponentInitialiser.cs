using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Droneheim.GUI
{
	interface TextStyler
	{
		void Style(Text text);
	}

	class BasicTextStyler : TextStyler
	{
		protected int size;
		protected FontStyle fontStyle;
		protected Color color;

		public BasicTextStyler(int size, FontStyle fontStyle, Color color)
		{
			this.size = size;
			this.fontStyle = fontStyle;
			this.color = color;
		}

		public void Style(Text text)
		{
			text.fontSize = size;
			text.fontStyle = fontStyle;
			text.color = color;
		}
	}

	interface BoxLayoutStyler
	{
		void Style(RectTransform rect);
		void Style(LayoutGroup layoutGroup);
	}

	class BasicBoxLayoutStyler : BoxLayoutStyler
	{
		public int Width { get; set; }
		public int Height { get; set; }
		public RectOffset Padding { get; set; }
		public int Spacing { get; set; }

		public void Style(RectTransform rect)
		{
			if (Width != 0 && Height != 0)
			{
				rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Width);
				rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Height);
				rect.ForceUpdateRectTransforms();
				//rect.sizeDelta = new Vector2(Width, Height);
			}
		}

		public void Style(LayoutGroup layoutGroup)
		{
			layoutGroup.padding = Padding;
		}
	}

	interface ImageStyler
	{
		void Style(Image image);
	}

	class BackgroundImageStyler : ImageStyler
	{
		protected Sprite Sprite { get; set; }
		public Color Color { get; }

		public BackgroundImageStyler(Sprite sprite, Color color)
		{
			Sprite = sprite;
			Color = color;
		}

		public void Style(Image image)
		{
			image.sprite = Sprite;
			image.type = Image.Type.Sliced;
			image.color = Color;
		}
	}

	class Styler
	{
		public Styler Parent { get; set; }
		public TextStyler TextStyler { get; set; }
		public ImageStyler ImageStyler { get; set; }
		public BoxLayoutStyler BoxLayoutStyler { get; set; }
	}

	public class ComponentInitialiser
	{
		private DefaultControls.Resources resources = new DefaultControls.Resources();

		static Color RGBColor(int r, int g, int b)
		{
			return new Color((float)r / 255.0f, (float)g / 255.0f, (float)b / 255.0f);
		}

		public ComponentInitialiser()
		{
		}

		public static void InitAnchors(RectTransform rect)
		{
			rect.anchorMin = Vector2.zero;
			rect.anchorMax = Vector2.one;
			rect.sizeDelta = Vector2.zero;
			rect.pivot = Vector2.zero;
		}

		public static void InitContentFitter(ContentSizeFitter fitter)
		{	
			fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
			fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
		}

		protected void InitialiseAnchors(RectTransform rect)
		{
			rect.anchorMin = Vector2.zero;
			rect.anchorMax = Vector2.one;
			rect.sizeDelta = Vector2.zero;
			rect.pivot = Vector2.zero;
		}
		
		public GameObject Text(string text, GameObject parent = null, string styleClass = null)
		{
			GameObject obj = DefaultControls.CreateText(resources);
			obj.AddComponent<StyledElement>().StyleClass = styleClass;
			Text textComponent = obj.GetComponent<Text>();
			textComponent.text = text;
			textComponent.alignment = TextAnchor.MiddleCenter;

			InitialiseAnchors(obj.GetComponent<RectTransform>());
			InitContentFitter(obj.AddComponent<ContentSizeFitter>());

			if (parent != null )
			{
				obj.transform.SetParent(parent.transform);
			}
			return obj;
		}

		static void AddStyleElement(GameObject obj, string type = null, string styleClass = null)
		{
			StyledElement e = obj.AddComponent<StyledElement>();
			e.StyleClass = styleClass;
			e.ElementType = type;
		}

		public GameObject Button(string text, GameObject parent = null, string styleClass = null)
		{
			GameObject buttonObject = new GameObject();
			buttonObject.AddComponent<RectTransform>();
			buttonObject.AddComponent<Button>();
			buttonObject.AddComponent<HorizontalLayoutGroup>();
			buttonObject.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
			AddStyleElement(buttonObject, "Button", styleClass);
			InitialiseAnchors(buttonObject.GetComponent<RectTransform>());

			Image imageComponent = buttonObject.AddComponent<Image>();

			GameObject textObject = new GameObject();
			textObject.transform.SetParent(buttonObject.transform);
			textObject.AddComponent<RectTransform>();
			AddStyleElement(textObject);
			InitialiseAnchors(textObject.GetComponent<RectTransform>());

			Text textComponent = textObject.AddComponent<Text>();
			//textComponent.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
			textComponent.text = text;
			textComponent.alignment = TextAnchor.MiddleCenter;
			textComponent.fontSize = 20;
			textComponent.color = Color.black;

			if (parent != null)
			{
				buttonObject.transform.SetParent(parent.transform);
			}

			return buttonObject;
		}

		public GameObject Layout(bool horizontal = false, string styleClass = null)
		{
			GameObject panel = new GameObject();
			panel.AddComponent<StyledElement>().StyleClass = styleClass;
			InitialiseAnchors(panel.AddComponent<RectTransform>());
			InitContentFitter(panel.AddComponent<ContentSizeFitter>());

			panel.AddComponent<Image>();
			HorizontalOrVerticalLayoutGroup layoutGroup;
			if (horizontal) layoutGroup = panel.AddComponent<HorizontalLayoutGroup>();
			else layoutGroup = panel.AddComponent<VerticalLayoutGroup>();
			layoutGroup.childControlHeight = false;
			layoutGroup.childForceExpandHeight = false;

			return panel;
		}

		public static GameObject Panel(string styleClass = null)
		{
			GameObject panel = new GameObject();
			panel.AddComponent<StyledElement>().StyleClass = styleClass;
			InitAnchors(panel.AddComponent<RectTransform>());

			Image imageComponent = panel.AddComponent<Image>();

			VerticalLayoutGroup verticalLayoutGroup = panel.AddComponent<VerticalLayoutGroup>();
			panel.AddComponent<ContentSizeFitter>();
			verticalLayoutGroup.childControlHeight = false;
			verticalLayoutGroup.childForceExpandHeight = false;

			return panel;
		}

		public static GameObject TextInput(string styleClass = null)
		{
			GameObject root = new GameObject();
			AddStyleElement(root, "Input", styleClass);

			InputField inputField = root.AddComponent<InputField>();
			Image image = root.AddComponent<Image>();
			root.AddComponent<ContentSizeFitter>();
			InitAnchors(root.GetComponent<RectTransform>());
			//root.GetComponent<RectTransform>().sizeDelta = new Vector2(100, 100);

			image.color = Color.red;

			GameObject text = new GameObject("Text");
			inputField.textComponent = text.AddComponent<Text>();
			text.transform.SetParent(inputField.transform);
			AddStyleElement(text);

			GameObject placeholder = new GameObject("Placeholder");
			inputField.placeholder = placeholder.AddComponent<Text>();
			placeholder.transform.SetParent(inputField.transform);
			AddStyleElement(placeholder);

			return root;
		}
	}
}
