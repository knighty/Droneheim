using UnityEngine;
using UnityEngine.UI;

namespace Droneheim.GUI
{
	[RequireComponent(typeof(VerticalLayoutGroup))]
	public class TimelineTrackLabel : MonoBehaviour
	{
		protected void Awake()
		{
			gameObject.AddComponent<StyledElement>().SetTypeClasses("Element", "timeline-track-label");

			LayoutElement layoutElement = gameObject.AddComponent<LayoutElement>();
			layoutElement.minWidth = 50;
			layoutElement.flexibleWidth = 0;
			layoutElement.flexibleHeight = 1;

			GameObject text = ComponentInitialiser.Text("1", gameObject, "label");
		}
	}
}
