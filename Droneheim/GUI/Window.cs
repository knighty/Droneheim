using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Droneheim.GUI
{
	internal class Window
	{
		private GameObject content;
		public GameObject Content { 
			get { return content; }
			set
			{
				content = value;
				content.transform.parent = contentObject.transform;
			}
		}

		public RectTransform PositionRect
		{
			get => rootObject.GetComponent<RectTransform>();
		}

		public Vector2 Position { 
			set
			{
				PositionRect.anchoredPosition = value;
			}
		}

		public Vector2 Size
		{
			set
			{
				PositionRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, value.x);
				//PositionRect.sizeDelta = value;
			}
		}

		public Vector2 AnchorMin
		{
			set
			{
				PositionRect.anchorMin = value;
			}
		}
		public Vector2 AnchorMax
		{
			set
			{
				PositionRect.anchorMax = value;
			}
		}

		public string Title
		{
			set { titleObject.GetComponent<Text>().text = value; }
		}

		private GameObject rootObject;
		private GameObject titleObject;
		private GameObject contentObject;

		public Window(string title)
		{
			rootObject = InitWindow();

			titleObject = InitTitle(title);
			titleObject.transform.SetParent(rootObject.transform);

			contentObject = new GameObject();
			contentObject.AddComponent<RectTransform>();
			contentObject.AddComponent<LayoutElement>().flexibleHeight = 1;
			contentObject.transform.SetParent(rootObject.transform);
			content = contentObject;

			{
				VerticalLayoutGroup verticalLayoutGroup = contentObject.AddComponent<VerticalLayoutGroup>();
				contentObject.GetComponent<RectTransform>().anchorMin = Vector2.zero;
				contentObject.GetComponent<RectTransform>().anchorMax = Vector2.zero;
				contentObject.GetComponent<RectTransform>().pivot = new Vector2(0, 0);
				contentObject.GetComponent<RectTransform>().sizeDelta = Vector2.zero;
				verticalLayoutGroup.childControlHeight = false;
				verticalLayoutGroup.childForceExpandHeight = false;
			}
		}

		protected GameObject InitWindow()
		{
			GameObject rootObject = ComponentInitialiser.Panel("Window");
			rootObject.GetComponent<VerticalLayoutGroup>().childControlHeight = true;
			rootObject.GetComponent<VerticalLayoutGroup>().spacing = 0;
			return rootObject;
		}

		protected GameObject InitTitle(string title)
		{
			GameObject background = ComponentInitialiser.Layout(false, "Title");
			background.GetComponent<VerticalLayoutGroup>().childControlHeight = true;
			background.GetComponent<VerticalLayoutGroup>().childControlWidth = true;
			background.AddComponent<LayoutElement>().flexibleHeight = 0;
			background.GetComponent<LayoutElement>().minHeight = 42;

			GameObject text = ComponentInitialiser.Text(title, background);
			text.AddComponent<LayoutElement>().flexibleHeight = 1;
			text.GetComponent<LayoutElement>().flexibleWidth = 1;

			return background;
		}

		public void AddTo(GameObject obj)
		{
			rootObject.transform.SetParent(obj.transform);
		}

		public void Destroy()
		{
			GameObject.Destroy(rootObject);
		}
	}
}
