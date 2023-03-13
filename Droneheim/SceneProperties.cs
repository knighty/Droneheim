using Droneheim.GUI.Properties;
using UnityEngine;

namespace Droneheim
{
	public class SceneProperties
	{
		[EditableProperty("Time Of Day", "Time of day where 0 = midnight and 0.5 = midday")]
		[FloatRange(-1, 1)]
		public BasicKeyframeList<float> TimeOfDay { get; } = new FloatKeyframes(0);

		[EditableProperty("Override Time Of Day")]
		public bool OverrideTimeOfDay { get; } = false;

		[EditableProperty("Time Scale", "Speed time progresses")]
		[FloatRange(0, 3)]
		public BasicKeyframeList<float> TimeScale { get; } = new FloatKeyframes(1);

		[EditableProperty("Environment", "Current environment (weather, etc)")]
		[Enum()]
		public BasicKeyframeList<string> Environment { get; } = new StringKeyframes("");

		[EditableProperty("Override Environment")]
		public bool OverrideEnvironment { get; } = false;

		[EditableProperty("Wind Strength", "How fast the wind is blowing")]
		[FloatRange(0, 1)]
		public BasicKeyframeList<float> WindStrength { get; } = new FloatKeyframes(35);

		[EditableProperty("Wind Direction", "The direction the wind is blowing")]
		public BasicKeyframeList<float> WindDirection { get; } = new FloatKeyframes(0);

		[EditableProperty("Override Wind")]
		public bool OverrideWind { get; } = false;
	}
}
