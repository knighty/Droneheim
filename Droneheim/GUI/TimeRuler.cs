using UnityEngine;
using UnityEngine.UI;

namespace Droneheim.GUI
{
	[RequireComponent(typeof(RectMask2D), typeof(RectTransform))]
	class TimeRuler : MonoBehaviour
	{
		const int MAX_ELEMENTS = 100;

		GameObject[] timeTextObjects = new GameObject[MAX_ELEMENTS];
		TimeRulerGraphic scrubberGraphic = null;
		TimelineGUI timeline;

		public void Awake()
		{
			RectTransform rect = GetComponent<RectTransform>();
			rect.anchorMin = Vector2.zero;
			rect.anchorMax = Vector2.one;
			rect.pivot = Vector2.zero;
			rect.sizeDelta = Vector2.zero;

			LayoutElement layoutElement = gameObject.AddComponent<LayoutElement>();
			layoutElement.minHeight = 30;
			layoutElement.preferredHeight = 30;

			InitTextObjects();

			scrubberGraphic = gameObject.AddComponent<TimeRulerGraphic>();

			GameObject scrubberObj = new GameObject();
			scrubberObj.transform.SetParent(transform, false);
			scrubberObj.AddComponent<TimeScrubber>();
		}

		protected void Start()
		{
			timeline = GetComponentInParent<TimelineGUI>();
			timeline.TimelineTransform.OnTransformChanged += TransformChanged;

			UpdateTextObjects();
		}

		protected void TransformChanged()
		{
			UpdateTextObjects();
		}

		private string FormatTime(int seconds)
		{
			int minutes = seconds / 60;
			seconds -= minutes * 60;

			return $"{minutes}:{seconds:00}";
		}

		public void InitTextObjects()
		{
			for (int i = 0; i < MAX_ELEMENTS; i++)
			{
				GameObject obj = ComponentInitialiser.Text(FormatTime(i), gameObject, "timeline-time");
				obj.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;
				timeTextObjects[i] = obj;
			}
		}

		public void UpdateTextObjects()
		{
			for (int i = 0; i < MAX_ELEMENTS; i++)
			{
				RectTransform rect = timeTextObjects[i].GetComponent<RectTransform>();
				rect.anchorMin = new Vector2(0, 1);
				rect.anchorMax = new Vector2(0, 1);
				//rect.sizeDelta = new Vector2(100, 30);
				rect.offsetMin = new Vector2(timeline.TimelineTransform.GetX(i * 30) - 50, -20);
				rect.offsetMax = new Vector2(timeline.TimelineTransform.GetX(i * 30) + 50, 0);
				rect.pivot = Vector2.zero;
				//rect.offsetMax = new Vector2(i * zoom + 50, -20);
			}
		}

		public void Update()
		{
		}
	}
}
