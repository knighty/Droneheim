using UnityEngine;
using UnityEngine.UI;

namespace Droneheim.GUI.Properties.Editors
{
	[EditablePropertyEditor(typeof(bool))]
	public class BoolEditor : SimplePropertyEditor<bool>
	{
		Toggle toggle;
		bool currentValue = false;

		protected override bool Value
		{
			set
			{
				currentValue = value;
				UpdateInput();
			}
			get
			{
				return currentValue;
			}
		}

		protected void UpdateInput()
		{
			toggle.isOn = currentValue;
		}

		protected override GameObject CreateEditorUI()
		{
			GameObject input = ComponentInitialiser.Checkbox();
			toggle = input.GetComponent<Toggle>();
			input.AddComponent<LayoutElement>().flexibleWidth = 0;
			input.GetComponent<LayoutElement>().minWidth = 20;
			input.GetComponent<LayoutElement>().minHeight = 20;

			return input;
		}
	}
}
