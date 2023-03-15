using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Droneheim.Commands
{
	public class SetKeyframe<T> : Command
	{
		BasicKeyframeList<T> keyframes;
		T value;
		int frame;

		bool hasPreviousValue;
		T previousValue;

		public override string Name => "Set Keyframe";

		public SetKeyframe(BasicKeyframeList<T> keyframes, int frame, T value)
		{
			this.keyframes = keyframes;
			this.value = value;
			this.frame = frame;

			hasPreviousValue = keyframes.HasKeyframeAt(frame);
			previousValue = keyframes.GetValueAt(frame);
		}

		public override void Execute()
		{
			keyframes.SetKeyframe(frame, value);
		}

		public override void Undo()
		{
			if (hasPreviousValue)
			{
				keyframes.SetKeyframe(frame, previousValue);
			}
			else
			{
				keyframes.RemoveKeyframe(frame);
			}
		}
	}
}
