using System;

namespace Droneheim.Timeline
{
	public class BlockTimelineController
	{
		protected TimelineController timeline;
		protected KeyframedBlock block;

		public int CurrentFrame
		{
			get
			{
				return timeline.PlaybackController.CurrentFrame;
			}
			set
			{
				timeline.PlaybackController.CurrentFrame = value;
			}
		}

		public Action<int> OnFrameChanged { get => timeline.PlaybackController.OnFrameChanged; set => timeline.PlaybackController.OnFrameChanged = value; }

		public BlockTimelineController(TimelineController timeline)
		{
			this.timeline = timeline;
		}

		public void GoTo(int frame)
		{
			CurrentFrame = frame;
		}
	}
}
