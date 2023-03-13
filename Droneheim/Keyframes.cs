using System;
using System.Collections.Generic;

namespace Droneheim
{
	public interface Keyframes
	{
		int Count { get; }
		Keyframe FirstKeyframe { get; }
		Keyframe LastKeyframe { get; }
		float MinimumMagnitude { get; }
		float MaximumMagnitude { get; }
		IEnumerable<Keyframe> Frames { get; }
		Action OnChange { get; set; }

		int GetNextKeyframe(int frame);
		int GetPreviousKeyframe(int frame);
		float GetMagnitudeAt(int frame);
		void MoveAll(int deltaFrames);
	}

	public interface Keyframe
	{
		int Frame { get; set; }
	}
}
