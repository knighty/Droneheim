using System;
using UnityEngine;

namespace Droneheim.GUI
{
	public class TimelineTransform
	{
		protected float zoom = 50;
		public float Zoom
		{
			get { return zoom; }
			set
			{
				zoom = value;
				OnTransformChanged?.Invoke();
				OnZoomChanged?.Invoke();
			}
		}

		protected float pan = 0;
		public float Pan
		{
			get { return pan; }
			set
			{
				pan = Mathf.Max(0, value);
				OnTransformChanged?.Invoke();
				OnPanChanged?.Invoke();
			}
		}


		public int GetX(int frame)
		{
			return (int)((frame * zoom) / 30 - pan);
		}

		public int GetXZoomed(int frame)
		{
			return (int)((frame * zoom) / 30);
		}

		public int GetFrame(int x)
		{
			return (int)((x + pan) / zoom * 30);
		}

		public int GetFrameZoomed(int x)
		{
			return (int)((x) / zoom * 30);
		}

		public Action OnTransformChanged;
		public Action OnPanChanged;
		public Action OnZoomChanged;
	}
}
