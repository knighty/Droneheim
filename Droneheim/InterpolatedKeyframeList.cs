namespace Droneheim
{
	abstract public class InterpolatedKeyframeList<Type> : BasicKeyframeList<Type>
	{
		abstract protected Interpolator<Type> Interpolator { get; }

		protected InterpolatedKeyframeList(Type defaultValue) : base(defaultValue)
		{
		}

		protected float HandleEasing(float time, EasingMode easingMode)
		{
			float cubic(float t)
			{
				return 3 * t * t - 2 * t * t * t;
			}
			switch (easingMode)
			{
				case EasingMode.Linear:
					return time;
				case EasingMode.EaseIn:
					return time >= 0.5f ? time : cubic(time);
				case EasingMode.EaseOut:
					return time >= 0.5f ? cubic(time) : time;
				case EasingMode.EaseBoth:
					return cubic(time);
			}
			return time;
		}

		public override Type GetValueAt(float frame)
		{
			if (list.Count == 0)
			{
				return this.value;
			}

			SplineKeyframe<Type> current = GetKeyframeAt(frame);
			if (current == null)
			{
				return list.First.Value;
			}
			SplineKeyframe<Type> next = current.Next;
			if (next == null)
			{
				return current.Value;
			}

			float d = next == current ? 1 : ((frame - current.Frame) / (next.Frame - current.Frame));
			return Interpolator.Interpolate(current.Value, next.Value, HandleEasing(d, next.Easing));
		}


	}
}
