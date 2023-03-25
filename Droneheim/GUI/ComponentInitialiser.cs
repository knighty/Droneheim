using Birta;
using System;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Droneheim.GUI
{
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
			rect.anchorMin = Vector2.zero;// new Vector2(0, 1);
			rect.anchorMax = Vector2.zero;// new Vector2(0, 1);
			rect.sizeDelta = Vector2.zero;
			rect.pivot = Vector2.zero;
		}

		public static void InitAnchors(RectTransform rect, Vector2 min, Vector2 max)
		{
			rect.anchorMin = min;
			rect.anchorMax = max;
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
		
		public static GameObject Text(string text, GameObject parent = null, string styleClass = null)
		{
			GameObject obj = new GameObject();
			obj.AddComponent<Text>();
			if (parent != null)
			{
				obj.transform.SetParent(parent.transform);
			}

			AddStyleElement(obj, "Element", styleClass);

			Text textComponent = obj.GetComponent<Text>();
			textComponent.text = text;
			textComponent.alignment = TextAnchor.UpperLeft;

			InitAnchors(obj.GetComponent<RectTransform>(), Vector2.zero, Vector2.one);

			
			return obj;
		}

		static void AddStyleElement(GameObject obj, string type = null, string styleClass = null)
		{
			StyledElement e = obj.AddComponent<StyledElement>();
			if (styleClass != null)
			{
				e.Classes.Add(styleClass);
			}
			e.ElementType = type;
		}

		public static GameObject Button(string text, GameObject parent = null, string styleClass = null)
		{
			GameObject buttonObject = new GameObject();
			Image imageComponent = buttonObject.AddComponent<Image>();
			
			buttonObject.AddComponent<HorizontalLayoutGroup>();
			buttonObject.GetComponent<HorizontalLayoutGroup>().childControlWidth = true;
			buttonObject.GetComponent<HorizontalLayoutGroup>().childForceExpandWidth = false;
			buttonObject.AddComponent<RectTransform>();
			buttonObject.AddComponent<Button>();

			AddStyleElement(buttonObject, "Button", styleClass);
			InitAnchors(buttonObject.GetComponent<RectTransform>());

			GameObject textObject = new GameObject();
			textObject.transform.SetParent(buttonObject.transform);
			textObject.AddComponent<RectTransform>();
			AddStyleElement(textObject);
			InitAnchors(textObject.GetComponent<RectTransform>());

			Text textComponent = textObject.AddComponent<Text>();
			//textComponent.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
			textComponent.text = text;
			textComponent.alignment = TextAnchor.MiddleCenter;

			if (parent != null)
			{
				buttonObject.transform.SetParent(parent.transform, false);
			}

			return buttonObject;
		}

		public static GameObject Layout(bool horizontal = false, string styleClass = null)
		{
			GameObject panel = new GameObject();
			AddStyleElement(panel, "Element", styleClass);

			panel.AddComponent<Image>().color = Color.clear;
			HorizontalOrVerticalLayoutGroup layoutGroup;
			if (horizontal) layoutGroup = panel.AddComponent<HorizontalLayoutGroup>();
			else layoutGroup = panel.AddComponent<VerticalLayoutGroup>();

			layoutGroup.childForceExpandHeight = false;
			layoutGroup.childForceExpandWidth = false;

			return panel;
		}

		public static GameObject Generic(string styleClass = null)
		{
			GameObject panel = new GameObject();
			//panel.transform.SetParent(parent.transform);
			AddStyleElement(panel, "Element", styleClass);
			InitAnchors(panel.AddComponent<RectTransform>());

			return panel;
		}

		public static GameObject Image(Sprite sprite, string styleClass = null, int size = -1)
		{
			GameObject img = ComponentInitialiser.Generic();
			Image image = img.AddComponent<Image>();
			image.sprite = sprite;
			image.type = UnityEngine.UI.Image.Type.Simple;
			LayoutElement layoutElement = img.AddComponent<LayoutElement>();
			layoutElement.minWidth = size == -1 ? sprite.rect.height : size;
			layoutElement.minHeight = size == -1 ? sprite.rect.width : size;
			layoutElement.preferredWidth = size == -1 ? sprite.rect.height : size;
			layoutElement.preferredHeight = size == -1 ? sprite.rect.width : size;

			return img;
		}

		public static GameObject Panel(string styleClass = null, bool layout = true)
		{
			GameObject panel = new GameObject();
			AddStyleElement(panel, "Element", styleClass);
			InitAnchors(panel.AddComponent<RectTransform>());

			Image imageComponent = panel.AddComponent<Image>();
			//imageComponent.color = Color.clear;

			if (layout)
			{
				VerticalLayoutGroup verticalLayoutGroup = panel.AddComponent<VerticalLayoutGroup>();
				verticalLayoutGroup.childControlHeight = true;
				verticalLayoutGroup.childForceExpandHeight = false;
			}

			return panel;
		}

		public static GameObject TextInput(string styleClass = null)
		{
			GameObject root = new GameObject();
			AddStyleElement(root, "Input", styleClass);

			InputField inputField = root.AddComponent<InputField>();
			Image image = root.AddComponent<Image>();
			InitAnchors(root.GetComponent<RectTransform>());

			GameObject text = new GameObject("Text");
			text.AddComponent<HorizontalLayoutGroup>();
			text.transform.SetParent(inputField.transform);
			inputField.textComponent = text.AddComponent<Text>();
			InitAnchors(text.GetComponent<RectTransform>(), Vector2.zero, Vector2.one);
			inputField.textComponent.alignment = TextAnchor.MiddleCenter;
			AddStyleElement(text);

			GameObject placeholder = new GameObject("Placeholder");
			placeholder.AddComponent<HorizontalLayoutGroup>();
			placeholder.transform.SetParent(inputField.transform);
			inputField.placeholder = placeholder.AddComponent<Text>();
			InitAnchors(placeholder.GetComponent<RectTransform>(), Vector2.zero, Vector2.one);
			AddStyleElement(placeholder);

			return root;
		}

		public static GameObject Checkbox(string styleClass = null)
		{
			GameObject root = new GameObject();
			AddStyleElement(root, "Element", styleClass);
			Toggle toggle = root.AddComponent<Toggle>();

			GameObject background = new GameObject();
			toggle.targetGraphic = background.AddComponent<Image>();
			background.transform.SetParent(root.transform);
			InitAnchors(background.GetComponent<RectTransform>(), Vector2.zero, Vector2.one);
			AddStyleElement(background, "Input");

			GameObject checkmark = new GameObject();
			toggle.graphic = checkmark.AddComponent<Image>();
			checkmark.GetComponent<Image>().sprite = DroneheimResources.Checkmark;
			checkmark.transform.SetParent(root.transform);
			InitAnchors(checkmark.GetComponent<RectTransform>(), Vector2.zero, Vector2.one);
			AddStyleElement(checkmark);

			return root;
		}
	}
}
