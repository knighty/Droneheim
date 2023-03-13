using System;
using System.Collections;
using System.Collections.Generic;

namespace Droneheim
{
	public interface KeyframeListMagnitude<T>
	{
		float ConvertValue(T value);
	}

	public class BasicKeyframeListFrameEnumerator<Type> : IEnumerator<Keyframe>
	{
		private SplineKeyframe<Type> current = null;
		private SplineKeyframe<Type> first = null;

		public BasicKeyframeListFrameEnumerator(SplineKeyframeList<Type> list)
		{
			first = list.First;
		}

		public Keyframe Current => current;

		object IEnumerator.Current => current.Frame;

		public void Dispose()
		{
		}

		public bool MoveNext()
		{
			if (current == null)
			{
				current = first;
			}
			else
			{
				current = current.Next;
			}
			return current != null;
		}

		public void Reset()
		{
			current = first;
		}
	}

	abstract public class BasicKeyframeList<Type> : Keyframes, IEnumerable<Keyframe>
	{
		protected SplineKeyframeList<Type> list = new SplineKeyframeList<Type>();
		public SplineKeyframeList<Type> List { get { return list; } }

		abstract protected KeyframeListMagnitude<Type> MagnitudeCalculator { get; }

		protected Type value;

		public int Count { get => list.Count; }
		public virtual float MinimumMagnitude { get => 0; }
		public virtual float MaximumMagnitude { get => 0; }
		public Keyframe FirstKeyframe { get => list.First; }
		public Keyframe LastKeyframe { get => list.Last; }

		private Action onChange;
		public Action OnChange { get => onChange; set { onChange = value; } }

		public IEnumerable<Keyframe> Frames => this;

		protected BasicKeyframeList(Type defaultValue)
		{
			value = defaultValue;
		}

		public IEnumerator<Keyframe> GetEnumerator()
		{
			return new BasicKeyframeListFrameEnumerator<Type>(list);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return new BasicKeyframeListFrameEnumerator<Type>(list);
		}

		protected SplineKeyframe<Type> GetKeyframeAt(float t)
		{
			SplineKeyframe<Type> keyframe = null;

			SplineKeyframe<Type> next = list.First;
			while (next != null)
			{
				if (next.Frame <= t)
				{
					keyframe = next;
				}
				next = next.Next;
			}

			return keyframe;
		}

		abstract public Type GetValueAt(float t);

		public void SetKeyframe(int t, Type value)
		{
			if (HasKeyframeAt(t))
			{
				SplineKeyframe<Type> keyframe = GetKeyframeAt(t);
				keyframe.Value = value;
			}
			else
			{
				SplineKeyframe<Type> keyframe = new SplineKeyframe<Type>(value, t, EasingMode.EaseBoth);
				list.Add(keyframe);
			}
			OnChange?.Invoke();
		}

		public void ToggleKeyframe(int t, Type value)
		{
			if (HasKeyframeAt(t))
			{
				list.RemoveAt(t);
			}
			else
			{
				SplineKeyframe<Type> keyframe = new SplineKeyframe<Type>(value, t, EasingMode.EaseBoth);
				list.Add(keyframe);
			}
			OnChange?.Invoke();
		}

		public void MoveKeyframe(int from, int to)
		{

		}

		public bool HasKeyframeAt(int frame)
		{
			if (list.Count == 0)
				return false;

			SplineKeyframe<Type> next = list.First;
			while (next != null)
			{
				if (next.Frame == frame)
					return true;
				next = next.Next;
			}

			return false;
		}

		public int GetNextKeyframe(int frame)
		{
			if (list.Count == 0)
			{
				return 0;
			}

			SplineKeyframe<Type> current = GetKeyframeAt(frame);
			return current.Next != null ? current.Next.Frame : 0;
		}

		public int GetPreviousKeyframe(int frame)
		{
			if (list.Count == 0)
			{
				return 0;
			}

			SplineKeyframe<Type> current = GetKeyframeAt(frame);
			if (current.Frame == frame)
			{
				if (current.Previous != null)
				{
					return current.Previous.Frame;
				}
			}
			return current != null ? current.Frame : 0;
		}

		public float GetMagnitudeAt(int frame)
		{
			return MagnitudeCalculator?.ConvertValue(GetValueAt((float)frame)) ?? 0;
		}

		public void MoveAll(int deltaFrames)
		{
			throw new NotImplementedException();
		}
	}
}
