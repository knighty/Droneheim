using Droneheim.Spline;
using UnityEngine;
using UnityEngine.UI;

namespace Droneheim.GUI.Properties.Editors
{
	[RequireComponent(typeof(StyledElement))]
	public class SplineNodeEditor : MonoBehaviour
	{
		public KeyframedEditableProperty<SplineNode> Property { get; set; }

		public void Start()
		{
			GameObject root = new GameObject();
			root.AddComponent<HorizontalLayoutGroup>();

			GameObject updateButton = new GameObject();
			updateButton.AddComponent<Button>();
			updateButton.transform.SetParent(root.transform);

			GameObject updateButtonText = new GameObject();
			updateButtonText.AddComponent<Text>().text = "Set";
			updateButtonText.transform.SetParent(updateButton.transform);

			root.transform.SetParent(gameObject.transform);
		}
	}

	/*public class SplineNodeFactory : Factory
	{
		public GameObject GetEditor(EditableProperty property)
		{
			GameObject editorObject = new GameObject();
			SplineNodeEditor editor = editorObject.AddComponent<SplineNodeEditor>();
			editor.Property = (KeyframedEditableProperty<SplineNode>)property;
			return editorObject;
		}
	}*/
}
