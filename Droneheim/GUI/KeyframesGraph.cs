using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Droneheim.Timeline;
using System.Linq;
using UnityEngine.Analytics;
using UnityEngine.EventSystems;
using System;

namespace Droneheim.GUI
{
	public class KeyframesGraph : MaskableGraphic, ILayoutElement, IDragHandler, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IEndDragHandler, IDeselectHandler
	{
		private List<KeyframedBlockKeyframes> blockKeyframes;
		public List<KeyframedBlockKeyframes> BlockKeyframes
		{
			get => blockKeyframes; set
			{
				blockKeyframes = value;
				SetAllDirty();
			}
		}

		private TimelineTransform timelineTransform;
		public TimelineTransform TimelineTransform { get => timelineTransform; set => timelineTransform = value; }

		protected KeyframesGraphLine lineGraph;
		protected KeyframesGraphPoints pointsGraph;

		public float minWidth => -1;
		public float preferredWidth => -1;
		public float flexibleWidth => 1;
		public float minHeight => 100;
		public float preferredHeight => 100;
		public float flexibleHeight => 0;
		public int layoutPriority => 1;

		protected enum State
		{
			Default,
			SelectingKeyframes,
			SelectedKeyframes,
			MovingKeyframes
		}

		protected State state = State.Default;
		protected Vector2 selectionStartPoint;
		protected HashSet<Keyframe> selectedKeyframes = new HashSet<Keyframe>();
		protected HashSet<Keyframe> selectingKeyframes = new HashSet<Keyframe>();

		public bool IsKeyframeSelected(Keyframe keyframe)
		{
			return selectedKeyframes.Contains(keyframe) || selectingKeyframes.Contains(keyframe);
		}

		protected override void Awake()
		{
			base.Awake();

			ComponentInitialiser.InitAnchors(GetComponent<RectTransform>(), Vector2.zero, Vector2.one);
			//GetComponent<RectTransform>().pivot = new Vector2(0, 1);

			GameObject lineObject = new GameObject();
			lineObject.transform.SetParent(transform);
			lineGraph = lineObject.AddComponent<KeyframesGraphLine>();

			GameObject pointsObject = new GameObject();
			pointsObject.transform.SetParent(transform);
			pointsGraph = pointsObject.AddComponent<KeyframesGraphPoints>();
		}

		protected void DrawKeyframes()
		{
			if (BlockKeyframes == null || TimelineTransform == null)
				return;

			Rect rect = GetPixelAdjustedRect();
			int lineResolution = 5;
			int numPoints = (int)Mathf.Ceil(rect.width / lineResolution);
			List<Vector2[]> linePoints = new List<Vector2[]>();
			Dictionary<Keyframe, Vector2> points = new Dictionary<Keyframe, Vector2>();

			foreach (var block in blockKeyframes)
			{
				Keyframes keyframes = block.Keyframes;
				if (keyframes.Count == 0) continue;

				float min = keyframes.MinimumMagnitude;
				float max = keyframes.MaximumMagnitude;
				float delta = max - min;
				Vector2[] lp = new Vector2[numPoints];

				for (int x = 0; x < numPoints; x++)
				{
					int frame = TimelineTransform.GetFrameZoomed(x * lineResolution);
					float value = keyframes.GetMagnitudeAt(frame);
					float y = delta > 0 ? ((value - min) / (max - min) * rect.height) : rect.height / 2;

					Vector2 center = new Vector2(x * lineResolution, y);
					lp[x] = center;
				}

				linePoints.Add(lp);

				foreach (var keyframe in keyframes.Frames)
				{
					int frame = keyframe.Frame;
					float x = TimelineTransform.GetXZoomed(frame);
					float value = keyframes.GetMagnitudeAt(frame);
					float y = delta > 0 ? ((value - min) / (max - min) * rect.height) : rect.height / 2;

					Vector2 center = new Vector2(x + rect.xMin, y + rect.yMin);
					points.Add(keyframe, center);
				}

				pointsGraph.Points = points;
			}

			lineGraph.Points = linePoints;
		}

		protected override void OnPopulateMesh(VertexHelper vh)
		{
			vh.Clear();
			DrawKeyframes();
		}

		public void HandleSelection(Rect selectionRect, ref HashSet<Keyframe> selected)
		{
			Rect rect = GetPixelAdjustedRect();
			foreach (var block in blockKeyframes)
			{
				Keyframes keyframes = block.Keyframes;
				if (keyframes.Count == 0) continue;

				float min = keyframes.MinimumMagnitude;
				float max = keyframes.MaximumMagnitude;
				float delta = max - min;

				foreach (var keyframe in keyframes.Frames)
				{
					int frame = keyframe.Frame;
					float x = TimelineTransform.GetXZoomed(frame);
					float value = keyframes.GetMagnitudeAt(frame);
					float y = delta > 0 ? ((value - min) / (max - min) * rect.height) : rect.height / 2;

					Vector2 center = new Vector2(x + rect.xMin, y + rect.yMin);
					if (selectionRect.Contains(center))
					{
						selected.Add(keyframe);
					}
				}
			}
		}

		protected void Update()
		{
		}

		protected void SetState(State newState)
		{
			EndState(state);
			state = newState;
			BeginState(state);
		}

		protected void BeginState(State state)
		{
			Debug.Log($"Begin State: {state}");
			switch(state)
			{
				case State.Default:
					selectedKeyframes.Clear();
					selectingKeyframes.Clear();
					pointsGraph.SetVerticesDirty();
					break;
				case State.SelectingKeyframes:
					if (!Input.GetKey(KeyCode.LeftShift))
					{
						Debug.Log("deselecting");
						selectedKeyframes.Clear();
					}
					selectingKeyframes.Clear();
					break;
			}
		}

		protected void EndState(State state)
		{
			switch (state)
			{
				case State.SelectingKeyframes:
					selectedKeyframes.UnionWith(selectingKeyframes);
					selectingKeyframes.Clear();
					pointsGraph.SetVerticesDirty();
					break;
				case State.SelectedKeyframes:
					pointsGraph.SetVerticesDirty();
					break;
			}
		}

		public void OnPointerEnter(PointerEventData eventData)
		{
		}

		public void OnPointerExit(PointerEventData eventData)
		{
		
		}

		public void OnBeginDrag(PointerEventData eventData)
		{
			switch (state)
			{
				case State.Default:
				case State.SelectedKeyframes:
					Vector2 local = new Vector2();
					RectTransformUtility.ScreenPointToLocalPointInRectangle(transform as RectTransform, eventData.position, null, out local);
					selectionStartPoint = local;
					SetState(State.SelectingKeyframes);
					break;
			}
		}

		public void OnDrag(PointerEventData eventData)
		{
			switch (state)
			{
				case State.SelectingKeyframes:
					Vector2 local = new Vector2();
					RectTransformUtility.ScreenPointToLocalPointInRectangle(transform as RectTransform, eventData.position, null, out local);
					float x = Mathf.Min(selectionStartPoint.x, local.x);
					float y = Mathf.Min(selectionStartPoint.y, local.y);
					float w = Mathf.Abs(selectionStartPoint.x - local.x);
					float h = Mathf.Abs(selectionStartPoint.y - local.y);
					Rect selectionRect = new Rect(x, y, w, h);
					selectingKeyframes.Clear();
					HandleSelection(selectionRect, ref selectingKeyframes);
					pointsGraph.SetVerticesDirty();
					break;
			}
		}

		public void OnEndDrag(PointerEventData eventData)
		{
			switch (state)
			{
				case State.SelectingKeyframes:
					SetState(State.SelectedKeyframes);
					break;
			}
		}

		public void OnDeselect(BaseEventData eventData)
		{
			selectedKeyframes.Clear();
			switch (state)
			{
				case State.SelectedKeyframes:
					SetState(State.Default);
					break;
			}
		}

		public void CalculateLayoutInputHorizontal()
		{
		}

		public void CalculateLayoutInputVertical()
		{
		}
	}
}
