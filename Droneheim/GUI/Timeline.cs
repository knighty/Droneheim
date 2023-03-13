using Droneheim.Timeline;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Droneheim.GUI
{
	public class TimelineGUI : MonoBehaviour, IScrollHandler, IDragHandler
	{
		public TimelineController timeline;
		public TimelineTransform TimelineTransform { get; set; } = new TimelineTransform();

		private GameObject playback;
		private GameObject tracksPanel;
		private GameObject timeRuler;

		private List<TimelineTrack> tracks = new List<TimelineTrack>();

		public int CurrentFrame
		{
			get => timeline.PlaybackController.CurrentFrame;
			set { timeline.PlaybackController.CurrentFrame = value; }
		}

		public void Awake()
		{
			InitUI();

			playback = InitPlayback();
			playback.transform.SetParent(transform, false);

			timeRuler = InitTimeRuler();
			timeRuler.transform.SetParent(transform, false);

			tracksPanel = InitTracksPanel();
			tracksPanel.transform.SetParent(transform, false);

			InitTracks(tracksPanel.transform, 3);
		}

		private void InitUI()
		{
			VerticalLayoutGroup layoutGroup = gameObject.AddComponent<VerticalLayoutGroup>();
			layoutGroup.childForceExpandHeight = false;

			LayoutElement layoutElement = gameObject.AddComponent<LayoutElement>();
			layoutElement.flexibleHeight = 1;
		}

		private GameObject InitPlayback()
		{
			GameObject obj = new GameObject();
			obj.AddComponent<Playback>();
			return obj;
		}

		private GameObject InitTimeRuler()
		{
			GameObject ruler = new GameObject();
			ruler.AddComponent<TimeRuler>();
			return ruler;
		}

		private GameObject InitTracksPanel()
		{
			GameObject panel = ComponentInitialiser.Layout(false, "block-panel");
			VerticalLayoutGroup layoutGroup = panel.GetComponent<VerticalLayoutGroup>();
			layoutGroup.childControlHeight = true;
			layoutGroup.childControlWidth = true;
			layoutGroup.childForceExpandHeight = false;
			layoutGroup.spacing = 8;
			panel.AddComponent<LayoutElement>().flexibleHeight = 1;

			return panel;
		}

		private void InitTracks(Transform parent, int num)
		{
			for(int i = 0; i < num; i++)
			{
				GameObject obj = new GameObject();
				TimelineTrack track = obj.AddComponent<TimelineTrack>();
				track.transform.SetParent(parent);
				tracks.Add(track);
			}
		}

		public void Start()
		{
			foreach (var block in timeline.Blocks)
			{
				tracks[block.Track].AddBlock(block);
			}
		}

		public void OnDrag(PointerEventData eventData)
		{
			if (eventData.dragging && eventData.button == PointerEventData.InputButton.Middle)
			{
				TimelineTransform.Pan -= eventData.delta.x;
			}
		}

		public void OnScroll(PointerEventData eventData)
		{
			TimelineTransform.Zoom += eventData.scrollDelta.y;
		}
	}
}
