using Droneheim.Spline;
using UnityEngine;

namespace Droneheim
{
	public class PlaybackController : MonoBehaviour
	{
		private float currentTime = 0;
		protected float playbackSpeed = 1;
		protected bool playing = false;

		public bool Playing { get => playing; set => playing = value; }

		public float CurrentFrame { get => currentTime * FrameRate; }
		public float CurrentTime { get => currentTime; }

		public int FrameRate
		{
			get { return 30; }
			set { }
		}

		public void Update()
		{
			float dt = Time.deltaTime;
			if (playing)
			{
				currentTime += dt * playbackSpeed;
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
			currentTime = 0;
		}
	}

	[RequireComponent(typeof(PlaybackController))]
	public class Timeline : MonoBehaviour
	{
		public PlaybackController PlaybackController 
		{ 
			get 
			{
				PlaybackController p = GetComponent<PlaybackController>();
				if (p == null)
				{
					p = gameObject.AddComponent<PlaybackController>();
				}
				return p;  
			} 
		}

		public float StartTime { get { return 0; } }
		public float EndTime { get { return 0; } }

		public int StartFrame { get { return 0; } }
		public int EndFrame { get { return 0; } }

		public void Start()
		{
			if (GetComponent<PlaybackController>() == null)
				gameObject.AddComponent<PlaybackController>();
		}

		public void Update()
		{
			//Player.m_localPlayer.Message(MessageHud.MessageType.TopLeft, "Playing Frame: " + playbackController.CurrentFrame);
		}
	}
}
