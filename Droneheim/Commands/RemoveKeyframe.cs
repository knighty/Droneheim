using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Droneheim.Commands
{
	public class RemoveKeyframe<T> : Command
	{
		BasicKeyframeList<T> keyframes;
		int frame;

		bool hasPreviousValue;
		T previousValue;

		public override string Name => "Remove Keyframe";

		public RemoveKeyframe(BasicKeyframeList<T> keyframes, int frame)
		{
			this.keyframes = keyframes;
			this.frame = frame;

			hasPreviousValue = keyframes.HasKeyframeAt(frame);
			previousValue = keyframes.GetValueAt(frame);
		}

		public override void Execute()
		{
			keyframes.RemoveKeyframe(frame);
		}

		public override void Undo()
		{
			if (hasPreviousValue)
			{
				keyframes.SetKeyframe(frame, previousValue);
			}
		}
	}
}
