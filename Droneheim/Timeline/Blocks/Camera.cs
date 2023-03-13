using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Droneheim.Timeline.Blocks
{
	public class Camera : KeyframedBlock
	{
		protected CameraProperties properties;

		public override string Name => "Camera";

		public override List<KeyframedBlockKeyframes> KeyframeLists
		{
			get => properties.GetType().GetProperties().
				Where(info => typeof(Keyframes).IsAssignableFrom(info.PropertyType)).
				Select(propertyInfo => new KeyframedBlockKeyframes((Keyframes)propertyInfo.GetValue(properties), propertyInfo.Name)).
				ToList();
		}

		public Camera(CameraProperties properties)
		{
			Track = 0;
			this.properties = properties;
		}

		protected override void HandleFrame(float frame)
		{

		}
	}
}
