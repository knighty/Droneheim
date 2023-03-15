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
	[RequireComponent(typeof(RectTransform))]
	public class KeyframesGraph : MonoBehaviour, ILayoutElement, IDragHandler, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IEndDragHandler, IDeselectHandler, ISelectHandler//, IPointerDownHandler
	{
		private List<KeyframedBlockKeyframes> blockKeyframes;
		public List<KeyframedBlockKeyframes> BlockKeyframes
		{
			get => blockKeyframes; set
			{
				blockKeyframes = value;
				UpdateGraphs();
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

		protected void Awake()
		{
			//base.Awake();

			ComponentInitialiser.InitAnchors(GetComponent<RectTransform>(), Vector2.zero, Vector2.one);
			//GetComponent<RectTransform>().pivot = new Vector2(0, 1);

			GameObject lineObject = new GameObject();
			lineObject.transform.SetParent(transform);
			lineGraph = lineObject.AddComponent<KeyframesGraphLine>();
			lineGraph.color = new Color32(255, 255, 255, 80);

			GameObject pointsObject = new GameObject();
			pointsObject.transform.SetParent(transform);
			pointsGraph = pointsObject.AddComponent<KeyframesGraphPoints>();
		}

		public Vector2 TransformPoint(Rect pixelAdjustedRect, Vector2 normalizedPoint)
		{
			return new Vector2(normalizedPoint.x, 5 + normalizedPoint.y * (pixelAdjustedRect.height - 10));
		}

		protected void UpdateGraphs()
		{
			if (BlockKeyframes == null || TimelineTransform == null)
				return;

			Rect rect = (transform as RectTransform).rect;
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
					float y = delta > 0 ? ((value - min) / (max - min)) : 0.5f;

					Vector2 center = TransformPoint(rect, new Vector2(x * lineResolution, y));
					lp[x] = center;
				}

				linePoints.Add(lp);

				foreach (var keyframe in keyframes.Frames)
				{
					int frame = keyframe.Frame;
					float x = TimelineTransform.GetXZoomed(frame);
					float value = keyframes.GetMagnitudeAt(frame);
					float y = delta > 0 ? ((value - min) / (max - min)) : 0.5f;

					Vector2 center = TransformPoint(rect, new Vector2(x + rect.xMin, y + rect.yMin));
					points.Add(keyframe, center);
				}

			}

			lineGraph.SetVerticesDirty();
			pointsGraph.Points = points;
		}

		/*protected override void OnPopulateMesh(VertexHelper vh)
		{
			vh.Clear();
			DrawKeyframes();
		}*/

		public void HandleSelection(Rect selectionRect, ref HashSet<Keyframe> selected)
		{
			Rect rect = (transform as RectTransform).rect;// GetPixelAdjustedRect();
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
					float y = delta > 0 ? ((value - min) / (max - min)) : 0.5f;

					Vector2 center = TransformPoint(rect, new Vector2(x + rect.xMin, y + rect.yMin));
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
			//Debug.Log($"Begin State: {state}");
			switch(state)
			{
				case State.Default:
					selectedKeyframes.Clear();
					selectingKeyframes.Clear();
					pointsGraph.SetVerticesDirty();
					break;
				case State.SelectingKeyframes:
					if (!Input.GetKey(KeyCode.LeftShift))
						selectedKeyframes.Clear();
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
			switch (state)
			{
				case State.SelectedKeyframes:
					selectedKeyframes.Clear();
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

		public void OnSelect(BaseEventData eventData)
		{
		}

		/*public void OnPointerDown(PointerEventData eventData)
		{
			if (eventData.button != PointerEventData.InputButton.Left)
				return;
			EventSystem.current.SetSelectedGameObject(gameObject, eventData);
		}*/
	}
}
