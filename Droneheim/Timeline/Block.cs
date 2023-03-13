using UnityEngine;

namespace Droneheim.Timeline
{
	public interface Block
	{
		int FrameStart { get; set; }
		int FrameEnd { get; }
		int Duration { get; }
		int Track { get; set; }

		string Name { get; }

		//void MoveBlock(int frame);

		//GameObject GetUI();
	}
}