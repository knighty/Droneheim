using Droneheim.GUI.Properties;
using Droneheim.Spline;
using UnityEngine;

namespace Droneheim
{
	public class CameraProperties
	{
		public class Shake
		{
			public Vector3 translationDisplacement = Vector3.one * 0.1f;
			public Vector3 rotationDisplacement = Vector3.one * 1.0f;
			public float speed = 0.15f;
		}

		public Shake cameraShake = new Shake();

		[EditableProperty("Transform", "The position and rotation of the camera")]
		public SplineKeyframes Transform { get; } = new SplineKeyframes(SplineNode.Identity);

		[EditableProperty("Aperture", "How closed the blades of the lens are (f-stop)")]
		[FloatRange(0.1f, 32.0f)]
		public BasicKeyframeList<float> Aperture { get; } = new FloatKeyframes(4);

		[EditableProperty("Focus Distance", "Distance between the lens and the object in focus (m)")]
		[FloatRange(0, 1000)]
		public BasicKeyframeList<float> FocusDistance { get; } = new FloatKeyframes(100);

		[EditableProperty("Focal Length", "Distance between the lens and the sensor where parallel rays converge (mm)")]
		[FloatRange(1, 600)]
		public BasicKeyframeList<float> FocalLength { get; } = new FloatKeyframes(35);

		[EditableProperty("Sensor Size", "The vertical size of the sensor (mm)")]
		[FloatRange(1, 60)]
		public BasicKeyframeList<float> SensorSize { get; } = new FloatKeyframes(35);

		[EditableProperty("Shake Amplitude", "The amount of camera shake")]
		[FloatRange(0, 10)]
		public BasicKeyframeList<float> ShakeAmplitude { get; } = new FloatKeyframes(1);

		[EditableProperty("Real Camera", "Whether to calculate focal length from FOV")]
		public bool RealCamera { get; set; }
	}
}
