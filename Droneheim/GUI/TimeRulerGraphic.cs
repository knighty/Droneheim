using UnityEngine;
using UnityEngine.UI;

namespace Droneheim.GUI
{
	class TimeRulerGraphic : MaskableGraphic
	{
		protected Color lineColor = new Color32(150, 150, 150, 255);
		protected Color sublineColor = new Color32(100, 100, 100, 255);

		TimelineGUI timeline;

		public TimeRulerGraphic()
		{
		}

		protected float Zoom { get => timeline.TimelineTransform.Zoom; }

		protected override void Awake()
		{
			base.Awake();
		}

		protected override void Start()
		{
			base.Start();
			timeline = GetComponentInParent<TimelineGUI>();
			timeline.TimelineTransform.OnTransformChanged += TransformChanged;
			TransformChanged();
		}

		protected void TransformChanged()
		{
			SetAllDirty();
		}

		protected override void OnPopulateMesh(VertexHelper vh)
		{
			Render(vh);
		}

		public void Render(VertexHelper vh)
		{
			vh.Clear();

			int thickness = 2;

			Rect rect = GetPixelAdjustedRect();
			vh.AddVert(new Vector3(rect.xMin, rect.yMin, 0), lineColor, Vector4.zero);
			vh.AddVert(new Vector3(rect.xMax, rect.yMin, 0), lineColor, Vector4.zero);
			vh.AddVert(new Vector3(rect.xMin, rect.yMin + thickness, 0), lineColor, Vector4.zero);
			vh.AddVert(new Vector3(rect.xMax, rect.yMin + thickness, 0), lineColor, Vector4.zero);

			vh.AddTriangle(0, 1, 2);
			vh.AddTriangle(1, 2, 3);

			RenderLargeSegments(vh, rect);
		}

		private void RenderLargeSegments(VertexHelper vh, Rect rect)
		{
			float width = 2;
			float height = 10;

			Vector3 Transform(Vector2 input, int frame)
			{
				return new Vector3(rect.xMin + timeline.TimelineTransform.GetX(frame) + input.x, rect.yMin + input.y, 0);
			}

			UIVertex InitVertex(bool subline = false)
			{
				UIVertex vertex = UIVertex.simpleVert;
				vertex.color = subline ? sublineColor : lineColor;
				return vertex;
			}

			UIVertex[] verts = new UIVertex[4];
			verts[0] = InitVertex();
			verts[1] = InitVertex();
			verts[2] = InitVertex();
			verts[3] = InitVertex();

			UIVertex[] subverts = new UIVertex[4];
			subverts[0] = InitVertex(true);
			subverts[1] = InitVertex(true);
			subverts[2] = InitVertex(true);
			subverts[3] = InitVertex(true);

			float subdivisionMinDistance = 10;
			int frameRate = 30;
			int subdivisions = (int)(Zoom / subdivisionMinDistance);
			while (frameRate % subdivisions != 0 && subdivisions > 0)
			{
				subdivisions--;
			}

			for (int second = 0; second < 100; second++)
			{
				verts[0].position = Transform(new Vector2(0, 0), second * 30);
				verts[1].position = Transform(new Vector2(0, height), second * 30);
				verts[2].position = Transform(new Vector2(width, height), second * 30);
				verts[3].position = Transform(new Vector2(width, 0), second * 30);
				vh.AddUIVertexQuad(verts);

				for (int j = 0; j < 30; j += 30 / subdivisions)
				{
					int frame = second * 30 + j;
					if (j == 0) continue;
					subverts[0].position = Transform(new Vector2(0, 2), frame);
					subverts[1].position = Transform(new Vector2(0, height / 2 + 2), frame);
					subverts[2].position = Transform(new Vector2(width / 2, height / 2 + 2), frame);
					subverts[3].position = Transform(new Vector2(width / 2, 2), frame);
					vh.AddUIVertexQuad(subverts);
				}
			}
		}

		public override void SetNativeSize()
		{
			SetAllDirty();
		}

		protected override void OnCanvasHierarchyChanged()
		{
			base.OnCanvasHierarchyChanged();
			SetAllDirty();
		}
	}
}
