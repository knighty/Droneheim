using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Droneheim.Timeline.Blocks
{
	internal class Scene : KeyframedBlock
	{
		protected SceneProperties sceneProperties = new SceneProperties();

		public override string Name => "Scene";

		public override List<KeyframedBlockKeyframes> KeyframeLists
		{
			get => sceneProperties.GetType().GetProperties().
				Where(info => typeof(Keyframe).IsAssignableFrom(info.GetType())).
				Select(propertyInfo => new KeyframedBlockKeyframes((Keyframes)propertyInfo.GetValue(sceneProperties), propertyInfo.Name)).
				ToList();
		}

		public Scene()
		{
			Track = 1;
		}

		protected override void HandleFrame(float frame)
		{
			SceneProperties p = sceneProperties;
			EnvMan env = EnvMan.instance;

			if (p.OverrideEnvironment)
			{
				env.m_debugEnv = p.Environment.GetValueAt(frame);
			}

			if (p.OverrideTimeOfDay)
			{
				env.m_debugTimeOfDay = true;
				env.m_debugTime = Mathf.Clamp01(p.TimeOfDay.GetValueAt(frame));
			}

			if (p.OverrideWind)
			{
				env.SetDebugWind(p.WindDirection.GetValueAt(frame), p.WindStrength.GetValueAt(frame));
			}
		}

		public override void EndPlayback()
		{
			EnvMan env = EnvMan.instance;
			env.m_debugEnv = "";
			env.m_debugTimeOfDay = false;
			env.ResetDebugWind();
		}
	}
}
