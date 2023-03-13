using System.Collections.Generic;
using UnityEngine;

namespace Droneheim.Timeline
{
	//[RequireComponent(typeof(PlaybackController))]
	public class TimelineController : MonoBehaviour
	{
		public PlaybackController PlaybackController
		{
			get
			{
				return GetComponent<PlaybackController>();
			}
		}

		public float StartTime { get { return 0; } }
		public float EndTime { get { return 0; } }

		public int StartFrame { get { return 0; } }
		public int EndFrame { get { return 0; } }

		public List<Block> Blocks { get; set; } = new List<Block>();

		public void Awake()
		{
			if (GetComponent<PlaybackController>() == null)
				gameObject.AddComponent<PlaybackController>();
		}

		public void Update()
		{
		}
	}
}
