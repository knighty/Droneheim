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
using Droneheim.GUI.Properties.Editors;
using UnityEngine.Analytics;

namespace Droneheim.GUI
{
	public class KeyframesGraphPoints : MaskableGraphic
	{
		protected Dictionary<Keyframe, Vector2> points;
		public Dictionary<Keyframe, Vector2> Points
		{
			get => points;
			set
			{
				SetVerticesDirty();
				points = value;
			}
		}

		public override Texture mainTexture => DroneheimResources.KeyframeOn.texture;

		private UIVertex[] verticesUnselected;
		private UIVertex[] verticesSelected;

		private KeyframesGraph graph;

		protected override void Awake()
		{
			base.Awake();

			graph = GetComponentInParent<KeyframesGraph>();

			Vector2[] uvsUnselected = DroneheimResources.KeyframeOff.uv;
			verticesUnselected = new UIVertex[4] {
				new UIVertex() { color = Color.white, uv0 = uvsUnselected[0] },
				new UIVertex() { color = Color.white, uv0 = uvsUnselected[2] },
				new UIVertex() { color = Color.white, uv0 = uvsUnselected[3] },
				new UIVertex() { color = Color.white, uv0 = uvsUnselected[1] },
			};
			Vector2[] uvsSelected = DroneheimResources.KeyframeOn.uv;
			verticesSelected = new UIVertex[4] {
				new UIVertex() { color = Color.white, uv0 = uvsSelected[0] },
				new UIVertex() { color = Color.white, uv0 = uvsSelected[2] },
				new UIVertex() { color = Color.white, uv0 = uvsSelected[3] },
				new UIVertex() { color = Color.white, uv0 = uvsSelected[1] },
			};

			ComponentInitialiser.InitAnchors(GetComponent<RectTransform>(), Vector2.zero, Vector2.one);
			GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
		}

		protected void Draw(VertexHelper vh)
		{
			if (Points == null)
				return;

			Vector2 size = new Vector2(16, 16);
			Vector2 halfSize = size / 2;

			foreach (var point in Points)
			{
				Keyframe keyframe = point.Key;
				Vector2 center = point.Value;

				UIVertex[] vertices = graph.IsKeyframeSelected(keyframe) ? verticesSelected : verticesUnselected;

				vertices[0].position = new Vector3(center.x - halfSize.x, center.y + halfSize.y);
				vertices[1].position = new Vector3(center.x - halfSize.x, center.y - halfSize.y);
				vertices[2].position = new Vector3(center.x + halfSize.x, center.y - halfSize.y);
				vertices[3].position = new Vector3(center.x + halfSize.x, center.y + halfSize.y);

				vh.AddUIVertexQuad(vertices);
			}
		}

		protected override void OnPopulateMesh(VertexHelper vh)
		{
			vh.Clear();
			Draw(vh);
		}
	}
}
