using System;
using System.Collections.Generic;
using System.Linq;

namespace Droneheim.Timeline
{
	public class KeyframedBlockKeyframes
	{
		public Keyframes Keyframes { get; set; }
		public string Name { get; set; }

		public KeyframedBlockKeyframes(Keyframes keyframes, string name)
		{
			Keyframes = keyframes;
			Name = name;
		}
	}

	public abstract class KeyframedBlock : Block
	{
		protected int frameStart = 0;
		public int FrameStart
		{
			get => CachedKeyframeLists.Aggregate(0, (min, list) => list.Keyframes.FirstKeyframe == null ? min : Math.Min(list.Keyframes.FirstKeyframe.Frame, min));
			set
			{
				foreach (var list in KeyframeLists)
				{
				}
			}
		}
		public int FrameEnd { get => CachedKeyframeLists.Aggregate(0, (max, list) => list.Keyframes.LastKeyframe == null ? max : Math.Max(list.Keyframes.LastKeyframe.Frame, max)); }
		public int Duration { get => FrameEnd - FrameStart; }

		public virtual string Name { get; }
		abstract public List<KeyframedBlockKeyframes> KeyframeLists { get; }
		public int Track { get; set; } = 0;

		private List<KeyframedBlockKeyframes> cachedKeyframeLists = null;
		private List<KeyframedBlockKeyframes> CachedKeyframeLists { get => cachedKeyframeLists ??= KeyframeLists; }

		private Action onChange;
		private bool subscribedToKeyframes = false;
		public Action OnChange
		{
			get
			{
				if (!subscribedToKeyframes)
				{
					foreach (var list in CachedKeyframeLists)
					{
						list.Keyframes.OnChange += KeyframeUpdated;
					}
					subscribedToKeyframes = true;
				}
				return onChange;
			}
			set
			{
				onChange = value;
				if (onChange == null)
				{
					foreach (var list in CachedKeyframeLists)
					{
						list.Keyframes.OnChange -= KeyframeUpdated;
					}
					subscribedToKeyframes = false;
				}
			}
		}

		protected void KeyframeUpdated()
		{
			OnChange?.Invoke();
		}

		abstract protected void HandleFrame(float frame);

		public virtual void EndPlayback() { }

		public void Update(int frame)
		{
			HandleFrame(frame - FrameStart);
		}
	}
}