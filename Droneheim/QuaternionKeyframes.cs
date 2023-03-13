using UnityEngine;

namespace Droneheim
{
	public class QuaternionKeyframes : InterpolatedKeyframeList<Quaternion>, Interpolator<Quaternion>
	{
		public QuaternionKeyframes(Quaternion defaultValue) : base(defaultValue)
		{
		}

		protected override Interpolator<Quaternion> Interpolator { get { return this; } }
		protected override KeyframeListMagnitude<Quaternion> MagnitudeCalculator => null;

		public Quaternion Empty { get { return Quaternion.identity; } }

		public Quaternion Interpolate(Quaternion a, Quaternion b, float t)
		{
			return Quaternion.Slerp(a, b, t);
		}
	}
}
