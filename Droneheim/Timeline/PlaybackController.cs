using System;
using UnityEngine;

namespace Droneheim.Timeline
{
	public class PlaybackController : MonoBehaviour
	{
		protected decimal playbackSpeed = 1;
		protected bool playing = false;

		public bool Playing { get => playing; set => playing = value; }

		protected decimal currentFrame = 0;
		public int CurrentFrame
		{
			set
			{
				currentFrame = value;
				OnFrameChanged?.Invoke(CurrentFrame);
			}
			get
			{
				return (int)currentFrame;
			}
		}

		public float CurrentTime { get => CurrentFrame / (float)FrameRate; }

		public int FrameRate
		{
			get { return 30; }
			set { }
		}

		public Action<int> OnFrameChanged { get; set; }

		public void Update()
		{
			float dt = Time.deltaTime;
			if (playing)
			{
				currentFrame += (decimal)dt * playbackSpeed * FrameRate;
				OnFrameChanged?.Invoke(CurrentFrame);
			}
		}

		public bool PlayPause()
		{
			if (playing)
			{
				Pause();
				return false;
			}
			else
			{
				Play();
				return true;
			}
		}

		public void Pause()
		{
			playing = false;
		}

		public void Play()
		{
			playing = true;
		}

		public void Stop()
		{
			playing = false;
			currentFrame = 0;
			OnFrameChanged?.Invoke(CurrentFrame);
		}
	}
}
