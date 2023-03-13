using System.Linq;
using UnityEngine;

namespace Droneheim
{
	public class FloatKeyframes : InterpolatedKeyframeList<float>, Interpolator<float>, KeyframeListMagnitude<float>
	{
		public FloatKeyframes(float defaultValue) : base(defaultValue)
		{
		}

		protected override Interpolator<float> Interpolator { get { return this; } }

		protected override KeyframeListMagnitude<float> MagnitudeCalculator => this;
		public override float MinimumMagnitude => List.Count == 0 ? 0 : List.Aggregate(float.MaxValue, (min, next) => Mathf.Min(min, next.Value));
		public override float MaximumMagnitude => List.Count == 0 ? 0 : List.Aggregate(float.MinValue, (max, next) => Mathf.Max(max, next.Value));

		public float Empty { get { return 0; } }

		public float Interpolate(float a, float b, float t)
		{
			return a * (1 - t) + b * t;
		}

		public float ConvertValue(float value)
		{
			return value;
		}
	}
}
