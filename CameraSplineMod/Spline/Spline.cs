using BepInEx.Configuration;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Droneheim.Spline
{
	public class SplineNode
	{
		public static SplineNode Identity { get { return new SplineNode(); } }
		public Vector3 Position { get; set; } = Vector3.zero;
		public Quaternion Rotation { get; set; } = Quaternion.identity;

		public SplineNode() { }
		public SplineNode(Transform transform)
		{
			Position = transform.position;
			Rotation = transform.rotation;
		}
	}

	public class SplineKeyframes : BasicKeyframeList<SplineNode>
	{
		CatmullRomCurve<Vector3> curvePosition = new CatmullRomCurve<Vector3>(new Vector3CatmullRomInterpolator(), 0.5f);
		CatmullRomCurve<Quaternion> curveRotation = new CatmullRomCurve<Quaternion>(new QuaternionCatmullRomInterpolator(), 0.5f);

		public SplineKeyframes(SplineNode defaultValue) : base(defaultValue)
		{
		}

		public override SplineNode GetValueAt(float t)
		{
			SplineKeyframe<SplineNode> a, b, c, d;

			b = GetKeyframeAt(t);

			if (b == null)
			{
				return this.value;
			}

			if (b.Next == null)
			{
				return b.Value;
			}

			a = b.Previous;
			c = b.Next;
			d = b.Next?.Next;

			float time = ((t - b.Time) / (c.Time - b.Time));

			Vector3 p0 = (a != null) ? a.Value.Position : b.Value.Position - (c.Value.Position - b.Value.Position);
			Vector3 p1 = b.Value.Position;
			Vector3 p2 = c.Value.Position;
			Vector3 p3 = (d != null) ? d.Value.Position : c.Value.Position + (c.Value.Position - b.Value.Position);

			Quaternion q0 = (a != null) ? a.Value.Rotation : b.Value.Rotation;
			Quaternion q1 = b.Value.Rotation;
			Quaternion q2 = c.Value.Rotation;
			Quaternion q3 = (d != null) ? d.Value.Rotation : c.Value.Rotation;

			return new SplineNode()
			{
				Position = curvePosition.Evaluate(
					time,
					p0, p1, p2, p3,
					a == null ? (b.Time - (c.Time - b.Time)) : a.Time,
					b.Time,
					c.Time,
					d == null ? (c.Time + (c.Time - b.Time)) : d.Time
				),
				Rotation = curveRotation.Evaluate(
					time,
					q0, q1, q2, q3,
					a == null ? (b.Time - (c.Time - b.Time)) : a.Time,
					b.Time,
					c.Time,
					d == null ? (c.Time + (c.Time - b.Time)) : d.Time
				)
			};
		}

		public void Clear()
		{
			list.Clear();
		}

		public Vector3 Empty { get { return Vector3.zero; } }
	}

	/*public class SplineFollower : MonoBehaviour
	{
		public Timeline Timeline { get; set; }
		public SplineKeyframes Spline { get; set; }

		public void Update()
		{
			SplineNode node = Spline.GetValueAt(Timeline.PlaybackController.CurrentFrame);
			this.transform.SetPositionAndRotation(node.Position, node.Rotation);
		}
	}*/

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
		public SplineKeyframes Spline { get; set; }

		public void OnDrawGizmos()
		{
			Debug.Log("Drawing gizmos");
			foreach (SplineKeyframe<SplineNode> keyframe in Spline.List)
			{
				Gizmos.DrawSphere(keyframe.Value.Position, 1);
			}
		}

		public SplineNode GetValueAt(float t)
		{
			return Spline.GetValueAt(t);
		}

		protected void RenderLine()
		{
			if (Spline == null)
				return;
			List<Vector3> positions = new List<Vector3>();
			for (float t = 0; t < Spline.GetLastKeyframe(); t += 1)
			{
				positions.Add(Spline.GetValueAt(t).Position);
			}
			LineRenderer.positionCount = positions.Count;
			LineRenderer.SetPositions(positions.ToArray());
		}

		public void Update()
		{
			RenderLine();
		}

		public void Start()
		{
			LineRenderer.material = new Material(Shader.Find("Standard"));		
		}
	}
}
