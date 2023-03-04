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
		private SplineKeyframe<Type> first;
		private SplineKeyframe<Type> last;
		public SplineKeyframe<Type> First { get => first; set => first = value; }
		public SplineKeyframe<Type> Last { get => last; set => last = value; }

		public int Count
		{
			get
			{
				if (first == null)
				{
					return 0;
				}
				SplineKeyframe<Type> n = first;
				int c = 0;
				while (n != null)
				{
					n = n.next;
					c++;
				}
				return c;
			}
		}

		public void Add(SplineKeyframe<Type> keyframe)
		{
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
				if (keyframe.Time > n.Time)
				{
					previous = n;
				}
				n = n.next;
			}

			if (previous == null)
			{
				first.previous = keyframe;
				keyframe.next = first;
				first = keyframe;
				return;
			}

			if (previous == last)
			{
				last = keyframe;
			}
			previous.next = keyframe;
			keyframe.previous = previous;
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
				last = keyframe;
			}
		}

		public void RemoveAt(int time)
		{
			SplineKeyframe<Type> n = first;
			while (n != null)
			{
				n = n.next;
				if (n.time == time)
				{
					Remove(n);
					return;
				}
			}
		}

		public void Clear()
		{
			first = null;
			last = null;
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
