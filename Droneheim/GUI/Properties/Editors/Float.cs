using Droneheim.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Droneheim.GUI.Properties.Editors
{
	[EditablePropertyEditor(typeof(BasicKeyframeList<float>), typeof(KeyframeEditorFactory))]
	public class KeyframedFloatEditor : KeyframedPropertyEditor<float, BasicKeyframeList<float>>
	{
		InputField inputField;
		float currentValue = 0;

		protected override float Value
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

		protected void ProcessInput(string str)
		{
			float val;
			if (float.TryParse(str, out val))
			{
				FloatRangeAttribute rangeAttribute = property.GetAttribute<FloatRangeAttribute>();
				if (rangeAttribute != null)
					val = rangeAttribute.Clamp(val);
				currentValue = val;
			}
		}

		protected void UpdateInput()
		{
			inputField.text = currentValue.ToString();
		}

		protected override GameObject CreateEditorUI()
		{
			GameObject input = ComponentInitialiser.TextInput();
			inputField = input.GetComponent<InputField>();
			inputField.contentType = InputField.ContentType.DecimalNumber;
			input.AddComponent<LayoutElement>().flexibleWidth = 0;
			input.GetComponent<LayoutElement>().preferredWidth = 70;

			inputField.onEndEdit.AddListener(text =>
			{
				ProcessInput(text);
				UpdateInput();
				SetKeyframe();
			});

			return input;
		}
	}
}
