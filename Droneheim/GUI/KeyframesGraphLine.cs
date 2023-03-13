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
		protected List<Vector2[]> points;
		public List<Vector2[]> Points
		{
			get => points;
			set
			{
				points = value;
			}
		}

		protected LineRenderer2D lineRenderer = new LineRenderer2D();

		protected override void Awake()
		{
			base.Awake();

			ComponentInitialiser.InitAnchors(GetComponent<RectTransform>(), Vector2.zero, Vector2.one);
			GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
		}

		protected void Draw(VertexHelper vh)
		{
			if (Points == null)
				return;

			LineRendererHelper lineRendererHelper = new LineRendererHelper(vh);
			foreach (var points in Points)
			{
				lineRenderer.Render(lineRendererHelper, points, new LineRendererOptions() { Weight = 1 });
			}
		}

		protected override void OnPopulateMesh(VertexHelper vh)
		{
			vh.Clear();
			Draw(vh);
		}
	}
}
