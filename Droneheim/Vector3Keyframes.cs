using UnityEngine;

namespace Droneheim
{
	public class Vector3Keyframes : InterpolatedKeyframeList<Vector3>, Interpolator<Vector3>
	{
		public Vector3Keyframes(Vector3 defaultValue) : base(defaultValue)
		{
		}

		public Vector3 Empty { get { return Vector3.zero; } }
		
		public Vector3 Interpolate(Vector3 a, Vector3 b, float t)
		{
			return a * (1 - t) + b * t;
		}

		public float ConvertValue(Vector3 value)
		{
			return 0;
		}

		protected override Interpolator<Vector3> Interpolator { get { return this; } }

		protected override KeyframeListMagnitude<Vector3> MagnitudeCalculator => null;
	}
}
