using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Droneheim.GUI
{
	public class SelectionBox : MaskableGraphic
	{
		private Rect rect;
		public Rect Rect
		{
			get => rect;
			set {
				rect = value;
				SetVerticesDirty();
			}
		}

		private int thickness = 2;

		protected void Draw(VertexHelper vh)
		{
			Vector2 bottomLeft = new Vector2(rect.xMin, rect.yMin);
			Vector2 bottomRight = new Vector2(rect.xMax, rect.yMin);
			Vector2 topLeft = new Vector2(rect.xMin, rect.yMax);
			Vector2 topRight = new Vector2(rect.xMax, rect.yMax);

			vh.AddVert(bottomLeft, color, Vector4.zero);
			vh.AddVert(bottomLeft + new Vector2(thickness, thickness), color, Vector4.zero);

			vh.AddVert(bottomRight, color, Vector4.zero);
			vh.AddVert(bottomRight + new Vector2(-thickness, thickness), color, Vector4.zero);

			vh.AddVert(topLeft, color, Vector4.zero);
			vh.AddVert(topLeft + new Vector2(thickness, -thickness), color, Vector4.zero);

			vh.AddVert(topRight, color, Vector4.zero);
			vh.AddVert(topRight + new Vector2(-thickness, -thickness), color, Vector4.zero);

			for (int i = 0; i < 4; i++)
			{
				vh.AddTriangle(i * 2 + 0, i * 2 + 1, i * 2 + 3);
				vh.AddTriangle(i * 2 + 0, i * 2 + 3, i * 2 + 2);
			}
		}

		protected override void OnPopulateMesh(VertexHelper vh)
		{
			vh.Clear();
			Draw(vh);
		}
	}
}
