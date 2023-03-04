using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

namespace Droneheim.GUI.Drawing
{
	class LineRendererHelper
	{
		int index = 0;
		VertexHelper vh;

		public LineRendererHelper(VertexHelper vh)
		{
			this.vh = vh;
		}

		public void AddTriangle(Vector3 p0, Vector3 p1, Vector3 p2, Color color)
		{
			vh.AddVert(p0, color, new Vector4(0, 0));
			vh.AddVert(p1, color, new Vector4(0, 1));
			vh.AddVert(p2, color, new Vector4(1, 0));

			vh.AddTriangle(index + 0, index + 1, index + 2);

			index += 3;
		}

		public int AddVert(Vector2 p0, Color color)
		{
			vh.AddVert(p0, color, new Vector4(0, 0));
			index++;
			return index - 1;
		}

		public void AddTriangle(int i0, int i1, int i2)
		{
			vh.AddTriangle(i0, i1, i2);
		}

		public void AddSegment(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, Color color)
		{
			vh.AddVert(p0, color, new Vector4(0, 0));
			vh.AddVert(p1, color, new Vector4(0, 1));
			vh.AddVert(p2, color, new Vector4(1, 0));
			vh.AddVert(p3, color, new Vector4(1, 1));

			vh.AddTriangle(index + 0, index + 2, index + 1);
			vh.AddTriangle(index + 2, index + 3, index + 1);

			index += 4;
		}
	}

	enum LineCap
	{
		Square,
		Rounded
	}

	enum LineJoinStyle
	{
		Miter,
		Bevel,
		Rounded
	}

	internal class LineRendererOptions
	{
		public LineCap LineCap = LineCap.Rounded;
		public float Weight = 1.0f;
	}

	internal class LineRenderer2D
	{
		protected void RenderEndpoint(LineRendererHelper helper, Vector2 p0, Vector2 t0, bool end, LineRendererOptions options)
		{
			t0 = end ? new Vector2(-t0.y, t0.x) : new Vector2(t0.y, -t0.x);
			t0 *= options.Weight;
			for (int i = 0; i < 32; i++)
			{
				float r1 = i / 32.0f * Mathf.PI;
				float r2 = (i + 1) / 32.0f * Mathf.PI;
				Vector2 p1 = p0 + new Vector2(t0.x * Mathf.Cos(r1) + t0.y * Mathf.Sin(r1), -t0.x * Mathf.Sin(r1) + t0.y * Mathf.Cos(r1));
				Vector2 p2 = p0 + new Vector2(t0.x * Mathf.Cos(r2) + t0.y * Mathf.Sin(r2), -t0.x * Mathf.Sin(r2) + t0.y * Mathf.Cos(r2));
				helper.AddTriangle(p0, p1, p2, Color.white);
			}
		}

		public void Render(LineRendererHelper helper, Vector2[] points, LineRendererOptions options)
		{
			if (points.Length > 1 && options.LineCap == LineCap.Rounded)
			{
				Vector2 t0 = points[1] - points[0];
				t0.Normalize();
				RenderEndpoint(helper, points[0], t0, false, options);
			}

			for (int i = 0; i < points.Length - 1; i++)
			{
				/*Vector2 p0 = points[i];
				Vector2 p1 = points[i + 1];
				Vector2 t = points[i + 1] - points[i];
				Vector2 t0 = i > 0 ? (points[i + 1] - points[i - 1]) : t;
				t0.Normalize();
				t0 *= options.Weight;
				Vector2 t1 = i < (points.Length - 2) ? (points[i + 2] - points[i]) : t;
				t1.Normalize();
				t1 *= options.Weight;

				helper.AddSegment(
					new Vector3(p0.x - t0.y, p0.y + t0.x, 0),
					new Vector3(p0.x + t0.y, p0.y - t0.x, 0),
					new Vector3(p1.x - t1.y, p1.y + t1.x, 0),
					new Vector3(p1.x + t1.y, p1.y - t1.x, 0),
					Color.white
				;*/

				bool firstPoint = i == 0;
				bool lastPoint = i < points.Length - 1;

				Vector2 p0 = points[i];
				
				Vector2 d0 = firstPoint ? p0 - points[i + 1] : (points[i - 1] - p0);
				d0.Normalize();
				Vector2 d1 = lastPoint ? d0 : points[i + 1] - p0;
				d1.Normalize();
				Vector2 halfway = (d0 + d1) / 2;
				halfway.Normalize();

				float sinAngle = Mathf.Abs(halfway.x * d0.y - halfway.y * d0.x);
				if (sinAngle == 0)
				{
					halfway.x = -d0.y;
					halfway.y = d0.x;
					sinAngle = 1;
				}

				Vector2 offset = halfway * options.Weight / sinAngle;
				float directionSign = Vector2.Dot(d0, new Vector2(-d1.y, d1.x)) > 0 ? -1 : 1;

				int index0 = helper.AddVert(p0 + offset * directionSign, Color.white);
				int index1 = helper.AddVert(p0 - offset * directionSign, Color.white);
				if (!lastPoint)
				{
					helper.AddTriangle(index0, index0 + 2, index0 + 1);
					helper.AddTriangle(index0 + 1, index0 + 2, index0 + 3);
				}
			}

			if (points.Length > 1)
			{
				Vector2 t0 = points[points.Length - 1] - points[points.Length - 2];
				t0.Normalize();
				RenderEndpoint(helper, points[points.Length - 1], t0, true, options);
			}
		}
	}
}
