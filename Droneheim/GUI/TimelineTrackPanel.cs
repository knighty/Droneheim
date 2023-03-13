using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Droneheim.GUI
{
	[RequireComponent(typeof(RectTransform), typeof(RectMask2D))]
	internal class TimelineTrackPanel : MonoBehaviour, ILayoutGroup, ILayoutElement
	{
		private float height = 200;

		public float minWidth => -1;

		public float preferredWidth => -1;

		public float flexibleWidth => 1;

		public float minHeight => height;

		public float preferredHeight => height;

		public float flexibleHeight => 0;

		public int layoutPriority => 2;

		protected void Awake()
		{
		}

		public void CalculateLayoutInputHorizontal()
		{
			BlockElement[] children = GetComponentsInChildren<BlockElement>();
			float height = 0;
			foreach (BlockElement child in children)
			{
				RectTransform rect = child.GetComponent<RectTransform>();
				float minHeight = LayoutUtility.GetMinHeight(rect);
				float preferredHeight = LayoutUtility.GetPreferredHeight(rect);
				height = Math.Max(preferredHeight, height);
			}
			this.height = height;
		}

		public void CalculateLayoutInputVertical()
		{
		}

		public void SetLayoutHorizontal()
		{
			BlockElement[] children = GetComponentsInChildren<BlockElement>();
			foreach (BlockElement child in children)
			{
				RectTransform rect = child.GetComponent<RectTransform>();
				float preferredWidth = LayoutUtility.GetPreferredSize(rect, 0);
				float preferredHeight = LayoutUtility.GetPreferredSize(rect, 1);
				rect.pivot = new Vector2(0, 1);
				rect.anchorMin = new Vector2(0, 1);
				rect.anchorMax = new Vector2(0, 1);
				rect.anchoredPosition = new Vector2(child.XPosition, 0);
				//rect.offsetMin = new Vector2(child.XPosition, 0);
				rect.sizeDelta = new Vector2(preferredWidth, preferredHeight);

				//rect.anchoredPosition = new Vector2(0, 0);
				//rect.sizeDelta = new Vector2(400, 100);

				//Debug.Log(rect.rect.ToString());
			}
		}

		public void SetLayoutVertical()
		{
		}
	}
}
