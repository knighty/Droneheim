using Droneheim;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Droneheim.Spline
{
	[RequireComponent(typeof(LineRenderer))]
	public class SplineRenderer : MonoBehaviour
	{
		protected LineRenderer lineRenderer = null;
		protected LineRenderer LineRenderer
		{
			get
			{
				if (lineRenderer == null)
				{
					lineRenderer = GetComponent<LineRenderer>();
				}
				return lineRenderer;
			}
		}

		protected SplineKeyframes spline;
		public SplineKeyframes Spline
		{
			set
			{
				spline = value;
				RenderLine();
				spline.OnChange += RenderLine;
			}
		}

		public SplineNode GetValueAt(float t)
		{
			return spline.GetValueAt(t);
		}

		protected void RenderLine()
		{
			if (spline == null)
				return;
			List<Vector3> positions = new List<Vector3>();
			for (float t = 0; t < (spline.LastKeyframe?.Frame ?? 0); t += 1)
			{
				positions.Add(spline.GetValueAt(t).Position);
			}
			LineRenderer.positionCount = positions.Count;
			LineRenderer.SetPositions(positions.ToArray());
		}

		public void Start()
		{
			LineRenderer.material = new Material(Shader.Find("Standard"));
		}

		public void OnDestroy()
		{
			if (spline != null)
				spline.OnChange -= RenderLine;
		}
	}
}
