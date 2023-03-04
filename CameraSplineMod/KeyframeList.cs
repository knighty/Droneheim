using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Droneheim
{
	public interface KeyframeList
	{
		int Count { get; }
		int GetNextKeyframe(int t);
		int GetPreviousKeyframe(int t);
		int GetFirstKeyframe();
		int GetLastKeyframe();
		float GetMagnitudeAt(int t);
		float GetMinimumMagnitude();
		float GetMaximumMagnitude();
		//IEnumerable<int> KeyframeTimes { get; }
	}

	abstract public class BasicKeyframeList<Type> : KeyframeList
	{
		protected SplineKeyframeList<Type> list = new SplineKeyframeList<Type>();
		public SplineKeyframeList<Type> List { get { return list; } }
		
		protected Type value;

		public int Count { get { return list.Count; } }

		//public IEnumerable<int> KeyframeTimes { get { return list.Select(x => x.Time); } }

		protected BasicKeyframeList(Type defaultValue)
		{
			value = defaultValue;
		}

		protected SplineKeyframe<Type> GetKeyframeAt(float t)
		{
			SplineKeyframe<Type> keyframe = null;

			SplineKeyframe<Type> next = list.First;
			while (next != null)
			{
				if (next.Time <= t)
				{
					keyframe = next;
				}
				next = next.Next;
			}

			return keyframe;
		}

		abstract public Type GetValueAt(float t);

		public void AddKeyframe(int t, Type value)
		{
			SplineKeyframe<Type> keyframe = new SplineKeyframe<Type>(value, t, EasingMode.EaseBoth);
			list.Add(keyframe);
		}

		public void RemoveKeyframe(int t)
		{

		}

		public int GetNextKeyframe(int t)
		{
			if (list.Count == 0)
			{
				return int.MaxValue;
			}

			SplineKeyframe<Type> current = GetKeyframeAt(t);
			return current.Next != null ? current.Next.Time : int.MaxValue;
		}

		public int GetPreviousKeyframe(int t)
		{
			if (list.Count == 0)
			{
				return 0;
			}

			SplineKeyframe<Type> current = GetKeyframeAt(t);
			return current != null ? current.Time : 0;
		}

		public float GetMagnitudeAt(int t)
		{
			return 0;
		}

		public float GetMinimumMagnitude()
		{
			return 0;
		}

		public float GetMaximumMagnitude()
		{
			return 100;
		}

		public int GetFirstKeyframe()
		{
			return (list.First != null ? list.First.Time : 0);
		}

		public int GetLastKeyframe()
		{
			return (list.Last != null ? list.Last.Time : 0);
		}
	}

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

		public override Type GetValueAt(float t)
		{
			if (list.Count == 0)
			{
				return this.value;
			}

			SplineKeyframe<Type> current = GetKeyframeAt(t);
			if (current == null)
			{
				return list.First.Value;
			}
			SplineKeyframe<Type> next = current.Next;
			if (next == null)
			{
				return current.Value;
			}

			float d = next == current ? 1 : ((t - current.Time) / (next.Time - current.Time));
			return Interpolator.Interpolate(current.Value, next.Value, HandleEasing(d, next.Easing));
		}
	}
}
