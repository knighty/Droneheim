using Droneheim.Timeline;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Droneheim.GUI
{
	[RequireComponent(typeof(VerticalLayoutGroup), typeof(Image))]
	public class BlockElement : MonoBehaviour, IDragHandler, IBeginDragHandler, ILayoutElement
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
		protected KeyframesLine keyframesLine;
		protected KeyframesGraph keyframesGraph;
		protected Dictionary<string, bool> keyframeVisibility = new Dictionary<string, bool>();
		protected TimelineTransform TimelineTransform { get; set; }

		protected void Awake()
		{
			gameObject.AddComponent<StyledElement>().SetTypeClasses("Element", "block");

			TimelineTransform = GetComponentInParent<TimelineGUI>().TimelineTransform;
			TimelineTransform.OnZoomChanged += UpdateTransform;

			/*GameObject lineObject = new GameObject();
			keyframesLine = lineObject.AddComponent<KeyframesLine>();
			lineObject.transform.SetParent(transform, false);*/

			GameObject lineObject = new GameObject();
			lineObject.transform.SetParent(transform, false);
			keyframesGraph = lineObject.AddComponent<KeyframesGraph>();
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

			return bar;
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

				/*List<Vector2> frames = new List<Vector2>();
				foreach(var list in block.KeyframeLists)
				{
					foreach(Keyframe keyframe in list.Keyframes.Frames)
					{
						frames.Add(new Vector2(TimelineTransform.GetXZoomed(keyframe.Frame), 0));
					}
				}
				keyframesLine.Keyframes = frames;*/

				keyframesGraph.BlockKeyframes = block.KeyframeLists;
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
	}
}