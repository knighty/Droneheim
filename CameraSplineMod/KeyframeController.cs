using Droneheim.Spline;
using UnityEngine;

namespace Droneheim
{
	public class KeyframeController
	{
		public SplineKeyframes spline = new SplineKeyframes(SplineNode.Identity);
		public FloatKeyframes fieldOfView = new FloatKeyframes(0.0f);

		public KeyframeList[] KeyframeLists
		{
			get { return new KeyframeList[] { spline, fieldOfView }; }
		}

		public int GetNextKeyframe(int t)
		{
			int next = 0;
			foreach (KeyframeList keyframeList in KeyframeLists)
			{
				int keyframeListNext = keyframeList.GetNextKeyframe(t);
				if (keyframeListNext > next)
				{
					next = keyframeListNext;
				}
			}
			return next;
		}

		public int GetPreviousKeyframe(int t)
		{
			int previous = int.MaxValue;
			foreach (KeyframeList keyframeList in KeyframeLists)
			{
				int keyframeListNext = keyframeList.GetPreviousKeyframe(t);
				if (keyframeListNext < previous)
				{
					previous = keyframeListNext;
				}
			}
			return previous;
		}
	}
}
