using Droneheim.Timeline;
using HarmonyLib;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Droneheim.GUI
{
	[RequireComponent(typeof(VerticalLayoutGroup), typeof(Image))]
	public class BlockElement : MonoBehaviour, IDragHandler, IBeginDragHandler, ILayoutElement, IDeselectHandler, ISelectHandler, IPointerDownHandler
	{
		private Block block = null;
		public Block Block
		{
			get
			{
				return block;
			}
			set
			{
				if (block is KeyframedBlock)
					((KeyframedBlock)block).OnChange -= UpdateTransform;

				block = value;
				text.text = block.Name;

				if (block is KeyframedBlock)
					((KeyframedBlock)block).OnChange += UpdateTransform;

				LayoutRebuilder.MarkLayoutForRebuild(transform as RectTransform);
			}
		}

		public int Track { get; set; } = 0;

		public int XPosition => block == null ? 0 : TimelineTransform.GetX(Block.FrameStart);

		public float minWidth => preferredWidth;
		public float preferredWidth => block == null ? 400 : Math.Max(100, (TimelineTransform.GetX(Block.FrameEnd) - TimelineTransform.GetX(Block.FrameStart)));
		public float flexibleWidth => 0;
		public float minHeight => -1;
		public float preferredHeight => -1;
		public float flexibleHeight => 0;
		public int layoutPriority => 2;

		protected Text text;
		protected KeyframesGraph keyframesGraph;
		protected Dictionary<string, bool> keyframeVisibility = new Dictionary<string, bool>();
		protected TimelineTransform TimelineTransform { get; set; }

		protected bool expanded = false;

		protected void Awake()
		{
			gameObject.AddComponent<StyledElement>().SetTypeClasses("Element", "block");

			TimelineTransform = GetComponentInParent<TimelineGUI>().TimelineTransform;
			TimelineTransform.OnZoomChanged += UpdateTransform;

			GameObject graph = new GameObject();
			graph.transform.SetParent(transform, false);
			keyframesGraph = graph.AddComponent<KeyframesGraph>();
			keyframesGraph.TimelineTransform = TimelineTransform;

			GameObject top = InitTop();
			top.transform.SetParent(transform);
		}

		private GameObject InitTop()
		{
			GameObject bar = new GameObject();
			HorizontalLayoutGroup layoutGroup = bar.AddComponent<HorizontalLayoutGroup>();
			layoutGroup.childForceExpandWidth = false;
			layoutGroup.childForceExpandHeight = false;

			LayoutElement layoutElement = bar.AddComponent<LayoutElement>();
			layoutElement.minHeight = 31;
			layoutElement.preferredHeight = 31;

			bar.AddComponent<StyledElement>().SetTypeClasses("Element", "label");

			GameObject textObject = ComponentInitialiser.Text("", bar);
			textObject.AddComponent<LayoutElement>().flexibleWidth = 1;
			textObject.GetComponent<LayoutElement>().flexibleHeight = 1;
			text = textObject.GetComponent<Text>();

			GameObject buttons = new GameObject();
			buttons.AddComponent<HorizontalLayoutGroup>();
			buttons.GetComponent<HorizontalLayoutGroup>().childAlignment = TextAnchor.MiddleRight;
			buttons.GetComponent<HorizontalLayoutGroup>().childForceExpandWidth = false;
			buttons.GetComponent<HorizontalLayoutGroup>().childForceExpandHeight = false;
			buttons.transform.SetParent(bar.transform, false);

			buttons.AddComponent<LayoutElement>().flexibleHeight = 1;

			GameObject eye = ComponentInitialiser.Image(DroneheimResources.Eye, null, 16);
			eye.transform.SetParent(buttons.transform);

			GameObject expand = ComponentInitialiser.Image(DroneheimResources.Expand, null, 16);
			expand.transform.SetParent(buttons.transform);
			expand.AddComponent<Button>().onClick.AddListener(ToggleKeyframeGraph);

			return bar;
		}

		private void ToggleKeyframeGraph()
		{
			expanded = !expanded;
			keyframesGraph.gameObject.SetActive(expanded);
			UpdateTransform();
		}

		public void Destroy()
		{
			TimelineTransform.OnZoomChanged -= UpdateTransform;
		}

		public void UpdateTransform()
		{
			LayoutRebuilder.MarkLayoutForRebuild(transform as RectTransform);

			if (block is KeyframedBlock)
			{
				KeyframedBlock block = (KeyframedBlock)Block;
				keyframesGraph.BlockKeyframes = block.CachedKeyframeLists;
			}
		}

		public void OnDrag(PointerEventData eventData)
		{
		}

		public void OnBeginDrag(PointerEventData eventData)
		{
		}

		public void CalculateLayoutInputHorizontal()
		{
		}

		public void CalculateLayoutInputVertical()
		{
		}

		public void OnDeselect(BaseEventData eventData)
		{
			GetComponent<StyledElement>().State &= ~StyleElementState.Focus;
			//GetComponentsInChildren<IDeselectHandler>().Do(o => o.OnDeselect(eventData));
			keyframesGraph.OnDeselect(eventData);
		}

		public void OnSelect(BaseEventData eventData)
		{
			Debug.Log("Selected Block Element");
			GetComponent<StyledElement>().State |= StyleElementState.Focus;
			keyframesGraph.OnSelect(eventData);
			//GetComponentsInChildren<ISelectHandler>().Do(o => o.OnSelect(eventData));
		}

		public void OnPointerDown(PointerEventData eventData)
		{
			if (eventData.button != PointerEventData.InputButton.Left)
				return;
			EventSystem.current.SetSelectedGameObject(gameObject, eventData);
		}
	}
}