using UnityEngine;

namespace Droneheim
{
	public class Vector2Keyframes : InterpolatedKeyframeList<Vector2>, Interpolator<Vector2>
	{
		public Vector2Keyframes(Vector2 defaultValue) : base(defaultValue)
		{
		}

		public Vector2 Interpolate(Vector2 a, Vector2 b, float t)
		{
			return a * (1 - t) + b * t;
		}

		protected override Interpolator<Vector2> Interpolator { get { return this; } }

		public Vector2 Empty { get { return Vector2.zero; } }

		protected override KeyframeListMagnitude<Vector2> MagnitudeCalculator => null;
	}
}
