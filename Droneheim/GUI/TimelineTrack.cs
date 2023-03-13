using Droneheim.Timeline;
using UnityEngine;
using UnityEngine.UI;

namespace Droneheim.GUI
{
	[RequireComponent(typeof(HorizontalLayoutGroup), typeof(Image))]
	public class TimelineTrack : MonoBehaviour
	{
		private TimelineTrackLabel label;
		private TimelineTrackPanel panel;

		protected void Awake()
		{
			InitUI();

			label = InitLabel();
			label.transform.SetParent(transform);

			panel = InitPanel();
			panel.transform.SetParent(transform);
		}

		private void InitUI()
		{
			gameObject.AddComponent<StyledElement>().SetTypeClasses("Element", "track");

			HorizontalLayoutGroup horizontalLayoutGroup = GetComponent<HorizontalLayoutGroup>();
			horizontalLayoutGroup.childControlWidth = true;
			horizontalLayoutGroup.childControlHeight = true;
			horizontalLayoutGroup.childForceExpandHeight = false;
			horizontalLayoutGroup.childForceExpandWidth = false;
		}

		private TimelineTrackLabel InitLabel()
		{
			GameObject label = new GameObject();
			return label.AddComponent<TimelineTrackLabel>();
		}

		private TimelineTrackPanel InitPanel()
		{
			GameObject obj = new GameObject();
			return obj.AddComponent<TimelineTrackPanel>();
		}

		public void AddBlock(Block block)
		{
			GameObject blockObject = new GameObject();
			blockObject.transform.SetParent(panel.transform);
			BlockElement blockElement = blockObject.AddComponent<BlockElement>();
			blockElement.Block = block;
		}
	}
}
