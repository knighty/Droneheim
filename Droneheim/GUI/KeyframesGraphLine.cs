using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine;
using UnityEngine.UI;
using Droneheim.Timeline;
using Droneheim.GUI.Drawing;
using Droneheim.Spline;

namespace Droneheim.GUI
{
	public class KeyframesGraphLine : MaskableGraphic
	{
		protected LineRenderer2D lineRenderer = new LineRenderer2D();

		private KeyframesGraph graph;
		protected KeyframesGraph Graph { get => graph ??= GetComponentInParent<KeyframesGraph>(); }

		protected override void Awake()
		{
			base.Awake();

			ComponentInitialiser.InitAnchors(GetComponent<RectTransform>(), Vector2.zero, Vector2.one);
			GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
		}

		protected void Draw(VertexHelper vh)
		{
			if (Graph.BlockKeyframes == null || Graph.TimelineTransform == null)
				return;

			Rect rect = (transform as RectTransform).rect;
			int lineResolution = 5;
			int numPoints = (int)Mathf.Ceil(rect.width / lineResolution);
			List<Vector2[]> linePoints = new List<Vector2[]>();

			foreach (var block in Graph.BlockKeyframes)
			{
				Keyframes keyframes = block.Keyframes;
				if (keyframes.Count == 0) continue;

				float min = keyframes.MinimumMagnitude;
				float max = keyframes.MaximumMagnitude;
				float delta = max - min;
				Vector2[] lp = new Vector2[numPoints];

				for (int x = 0; x < numPoints; x++)
				{
					int frame = Graph.TimelineTransform.GetFrameZoomed(x * lineResolution);
					float value = keyframes.GetMagnitudeAt(frame);
					float y = delta > 0 ? ((value - min) / (max - min)) : 0.5f;

					Vector2 center = Graph.TransformPoint(rect, new Vector2(x * lineResolution, y));
					lp[x] = center;
				}

				linePoints.Add(lp);
			}

			LineRendererHelper lineRendererHelper = new LineRendererHelper(vh);
			foreach (var p in linePoints)
			{
				lineRenderer.Render(lineRendererHelper, p, new LineRendererOptions() { Weight = 1, Color = color });
			}
		}

		protected override void OnPopulateMesh(VertexHelper vh)
		{
			vh.Clear();
			Draw(vh);
		}
	}
}
