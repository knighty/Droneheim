using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Droneheim.GUI
{
	class TimeScrubber : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
	{
		protected TimelineGUI timeline;
		protected Color scrubberColor = new Color32(136, 240, 0, 255);
		RectTransform rect;

		public TimeScrubber()
		{
		}

		bool dragging = false;

		public void OnBeginDrag(PointerEventData eventData)
		{
			dragging = true;
		}

		public void OnDrag(PointerEventData eventData)
		{
			Vector2 local;
			RectTransformUtility.ScreenPointToLocalPointInRectangle(timeline.GetComponent<RectTransform>(), eventData.position, null, out local);
			//Debug.Log(local);
			timeline.CurrentFrame = timeline.TimelineTransform.GetFrame((int)local.x);

			//Debug.Log(eventData.hovered.Aggregate("", (a,b) => a + " " + b.name));
		}

		public void OnEndDrag(PointerEventData eventData)
		{
			dragging = false;
		}

		protected void Awake()
		{
			Image image = gameObject.AddComponent<Image>();
			image.sprite = DroneheimResources.TimelineScrubber;

			rect = GetComponent<RectTransform>();
			ComponentInitialiser.InitAnchors(rect);
			rect.pivot = new Vector2(0.5f, 0);
			rect.sizeDelta = image.sprite.rect.size;

			//timeline.timeline.PlaybackController.OnFrameChanged += frame => Update();
		}

		protected void Start()
		{
			timeline = GetComponentInParent<TimelineGUI>();
		}

		protected void Update()
		{
			rect.anchoredPosition = new Vector2(timeline.TimelineTransform.GetX(timeline.CurrentFrame), -10);
			//rect.offsetMax = new Vector2(timeline.GetXFromFrame(timeline.CurrentFrame) + 20, 0);
		}
	}
}
