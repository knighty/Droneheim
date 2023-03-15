using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

		abstract public Type GetValueAt(float frame);

		public void SetKeyframe(int frame, Type value)
		{
			if (HasKeyframeAt(frame))
			{
				SplineKeyframe<Type> keyframe = GetKeyframeAt(frame);
				keyframe.Value = value;
			}
			else
			{
				SplineKeyframe<Type> keyframe = new SplineKeyframe<Type>(value, frame, EasingMode.EaseBoth);
				list.Add(keyframe);
			}
			OnChange?.Invoke();
		}

		public void RemoveKeyframe(int frame)
		{
			if (HasKeyframeAt(frame))
			{
				list.RemoveAt(frame);
			}
			OnChange?.Invoke();
		}

		public void ToggleKeyframe(int frame, Type value)
		{
			if (HasKeyframeAt(frame))
			{
				list.RemoveAt(frame);
			}
			else
			{
				SplineKeyframe<Type> keyframe = new SplineKeyframe<Type>(value, frame, EasingMode.EaseBoth);
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
