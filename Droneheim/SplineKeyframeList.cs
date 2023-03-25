using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Droneheim
{
	public class SplineKeyframeListEnumerator<Type> : IEnumerator<SplineKeyframe<Type>>
	{
		private SplineKeyframe<Type> current = null;
		private SplineKeyframe<Type> first = null;

		public SplineKeyframeListEnumerator(SplineKeyframeList<Type> list)
		{
			first = list.First;
		}

		public SplineKeyframe<Type> Current => current;

		object IEnumerator.Current => current;

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

	public class SplineKeyframeList<Type> : IEnumerable<SplineKeyframe<Type>>
	{
		protected int count = 0;
		private SplineKeyframe<Type> first;
		private SplineKeyframe<Type> last;

		public SplineKeyframe<Type> First { get => first; }
		public SplineKeyframe<Type> Last { get => last; }
		public int Count { get => count; }

		public void Add(SplineKeyframe<Type> keyframe)
		{
			count++;

			// Empty list
			if (first == null)
			{
				first = keyframe;
				last = keyframe;
				return;
			}

			SplineKeyframe<Type> previous = null;
			SplineKeyframe<Type> n = first;
			while (n != null)
			{
				if (keyframe.Frame > n.Frame)
				{
					previous = n;
				}
				n = n.next;
			}

			// Start
			if (previous == null)
			{
				first.previous = keyframe;
				keyframe.next = first;
				first = keyframe;
				return;
			}

			// End
			if (previous == last)
			{
				last = keyframe;
				previous.next = keyframe;
				keyframe.previous = previous;
				return;
			}

			// Middle
			SplineKeyframe<Type> next = previous.next;
			previous.next = keyframe;
			keyframe.previous = previous;
			keyframe.next = next;
			next.previous = keyframe;
		}

		public void Remove(SplineKeyframe<Type> keyframe)
		{
			if (keyframe.previous != null)
			{
				keyframe.previous.next = keyframe.next;
			}
			else
			{
				first = keyframe.next;
			}
			if (keyframe.next != null)
			{
				keyframe.next.previous = keyframe.previous;
			}
			else
			{
				last = keyframe.previous;
			}
			count--;
		}

		public void RemoveAt(int frame)
		{
			foreach(var keyframe in this)
			{
				if (keyframe.Frame == frame)
				{
					Remove(keyframe);
					break;
				}
			}
		}

		public void Clear()
		{
			first = null;
			last = null;
			count = 0;
		}

		public IEnumerator<SplineKeyframe<Type>> GetEnumerator()
		{
			return new SplineKeyframeListEnumerator<Type>(this);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return new SplineKeyframeListEnumerator<Type>(this);
		}
	}
}
