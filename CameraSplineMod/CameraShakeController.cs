using System;
using UnityEngine;

namespace Droneheim
{
	public class CameraShakeController
	{
		public Vector3 GetTranslation(CameraProperties cameraProperties, float t)
		{
			t *= cameraProperties.cameraShake.speed;
			Vector3 translation = Perlin.Sample3D(t, 0);
			return Vector3.Scale(translation, cameraProperties.cameraShake.translationDisplacement);
		}

		public Quaternion GetRotation(CameraProperties cameraProperties, float t)
		{
			t *= cameraProperties.cameraShake.speed;
			return Quaternion.Euler(
				Vector3.Scale(Perlin.Sample3D(t, 21), cameraProperties.cameraShake.rotationDisplacement)
			);
		}
	}
}
