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

		public Window(ComponentInitialiser initialiser, string title)
		{
			rootObject = ComponentInitialiser.Panel("Window");
			rootObject.GetComponent<VerticalLayoutGroup>().childControlHeight = true;

			GameObject titleBackground = initialiser.Layout(false, "Title");
			/*titleBackground.GetComponent<RectTransform>().anchorMin = new Vector2(1, 0);
			titleBackground.GetComponent<RectTransform>().anchorMax = new Vector2(1, 0);*/
			titleBackground.GetComponent<VerticalLayoutGroup>().childControlHeight = true;
			titleBackground.transform.SetParent(rootObject.transform);
			titleBackground.AddComponent<LayoutElement>().flexibleHeight = 0;

			titleObject = initialiser.Text(title, titleBackground);

			contentObject = new GameObject();
			contentObject.AddComponent<RectTransform>();
			contentObject.AddComponent<LayoutElement>().flexibleHeight = 1;
			contentObject.transform.SetParent(rootObject.transform);
			content = contentObject;

			contentObject.AddComponent<Image>().color = new Color(1, 0, 0, 0.2f);

			{
				VerticalLayoutGroup verticalLayoutGroup = contentObject.AddComponent<VerticalLayoutGroup>();
				contentObject.GetComponent<RectTransform>().anchorMin = Vector2.zero;
				contentObject.GetComponent<RectTransform>().anchorMax = Vector2.one;
				contentObject.GetComponent<RectTransform>().pivot = new Vector2(0, 0);
				contentObject.GetComponent<RectTransform>().sizeDelta = Vector2.zero;
				verticalLayoutGroup.childScaleWidth = true;
				verticalLayoutGroup.childControlHeight = false;
				verticalLayoutGroup.childForceExpandHeight = false;
			}

			/*GameObject graphObject = new GameObject();
			LineGraph graph = new LineGraph();
			GraphicGraphView graphView = graphObject.AddComponent<GraphicGraphView>();
			graphView.Graph = graph;
			graph.AddData(new GraphDataTest());
			graphObject.transform.SetParent(contentObject.transform);*/

			for (int i = 0; i < 3; i++)
			{
				//initialiser.Text("Test Button", contentObject, "Button");
				initialiser.Button("Test Button", contentObject, "Button");
			}
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
