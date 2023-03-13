using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Droneheim.GUI
{
	public class KeyframesLine : MaskableGraphic, ILayoutElement
	{
		protected List<Vector2> keyframes;
		public List<Vector2> Keyframes
		{
			get => keyframes;
			set
			{
				keyframes = value;
				SetAllDirty();
			}
		}

		public float minWidth => -1;
		public float preferredWidth => -1;
		public float flexibleWidth => 1;
		public float minHeight => 50;
		public float preferredHeight => 50;
		public float flexibleHeight => 0;
		public int layoutPriority => 1;

		protected override void Awake()
		{
			base.Awake();
		}

		protected void DrawKeyframes(VertexHelper vh)
		{
			if (Keyframes == null)
				return;

			Sprite sprite = DroneheimResources.KeyframeOn;
			Rect rect = GetPixelAdjustedRect();
			Vector2 size = new Vector2(16, 16);
			Vector2 halfSize = size / 2;
			Vector2[] uvs = sprite.uv;
			foreach (var keyframe in keyframes)
			{
				Vector2 center = new Vector2(keyframe.x + rect.xMin, keyframe.y + rect.yMin + rect.height / 2);
				//Debug.Log(center);
				vh.AddUIVertexQuad(
					new UIVertex[4] {
						new UIVertex() { position = new Vector3(center.x - halfSize.x, center.y + halfSize.y), color = Color.white, uv0 = uvs[0] },
						new UIVertex() { position = new Vector3(center.x - halfSize.x, center.y - halfSize.y), color = Color.white, uv0 = uvs[2] },
						new UIVertex() { position = new Vector3(center.x + halfSize.x, center.y - halfSize.y), color = Color.white, uv0 = uvs[3] },
						new UIVertex() { position = new Vector3(center.x + halfSize.x, center.y + halfSize.y), color = Color.white, uv0 = uvs[1] },
					}
				);
			}
		}

		public override Texture mainTexture
		{
			get
			{
				return DroneheimResources.KeyframeOn.texture;
			}
		}

		protected override void OnPopulateMesh(VertexHelper vh)
		{
			Color lineColor = Color.white;

			vh.Clear();
			int thickness = 2;

			Rect rect = GetPixelAdjustedRect();

			DrawKeyframes(vh);
		}

		public void CalculateLayoutInputHorizontal()
		{
		}

		public void CalculateLayoutInputVertical()
		{
		}
	}
}
