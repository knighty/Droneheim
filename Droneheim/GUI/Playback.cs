using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Droneheim.GUI
{
	[RequireComponent(typeof(HorizontalLayoutGroup))]
	public class Playback : MonoBehaviour
	{
		protected void Awake()
		{
			GetComponent<HorizontalLayoutGroup>().childForceExpandWidth = false;

			GameObject previous = ComponentInitialiser.Image(DroneheimResources.Previous);
			previous.transform.SetParent(transform);

			GameObject play = ComponentInitialiser.Image(DroneheimResources.Play);
			play.transform.SetParent(transform);

			GameObject next = ComponentInitialiser.Image(DroneheimResources.Next);
			next.transform.SetParent(transform);
		}
	}
}
