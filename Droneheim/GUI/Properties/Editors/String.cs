using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;
using UnityEngine;

namespace Droneheim.GUI.Properties.Editors
{
	[EditablePropertyEditor(typeof(BasicKeyframeList<string>), typeof(KeyframeEditorFactory))]
	public class KeyframedStringEditor : KeyframedPropertyEditor<string, BasicKeyframeList<string>>
	{
		InputField inputField;

		protected override string Value
		{
			set
			{
				inputField.text = value;
			}
			get
			{
				return inputField.text;
			}
		}

		protected override GameObject CreateEditorUI()
		{
			GameObject input = ComponentInitialiser.TextInput();
			inputField = input.GetComponent<InputField>();
			return input;
		}
	}
}
