using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;
using UnityEngine;

namespace Droneheim.GUI.Drawing
{
	class PointRendererHelper
	{
		int index = 0;
		VertexHelper vh;

		public PointRendererHelper(VertexHelper vh)
		{
			this.vh = vh;
		}

		public void AddCircle(Vector2 p0, float radius, Color color)
		{
			vh.AddVert(p0, color, new Vector4(0, 0));
			int startIndex = index;
			index += 1;

			int roundness = 32;
			for (int i = 0; i < roundness; i++)
			{
				float r = i / (float)roundness * Mathf.PI * 2;
				vh.AddVert(p0 + new Vector2(Mathf.Cos(r), Mathf.Sin(r)), color, new Vector4(0, 1));
				int vertIndex = startIndex + i + 1;
				vh.AddTriangle(startIndex, vertIndex, (i == roundness - 1) ? (startIndex + 1) : vertIndex);
			}
			index += roundness;
		}
	}

	enum PointShape
	{
		Square,
		Circle
	}

	internal class PointRendererOptions
	{
		public PointShape PointShape = PointShape.Circle;
		public float Weight = 1.0f;
	}

	internal class PointRenderer2D
	{
		public void Render(PointRendererHelper helper, Vector2[] points, PointRendererOptions options)
		{
			for (int i = 0; i < points.Length - 1; i++)
			{
				Vector2 p0 = points[i];
				helper.AddCircle(p0, options.Weight, Color.white);
			}
		}

		public void Render(PointRendererHelper helper, IEnumerable<Vector2> points, PointRendererOptions options)
		{
			foreach(Vector2 p0 in points)
			{
				helper.AddCircle(p0, options.Weight, Color.white);
			}
		}
	}
}
