using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Droneheim.GUI.Properties.Editors
{
	public class Float : SimplePropertyEditor<float>
	{
		public override float ConvertFromString(string str)
		{
			return float.Parse(str);
		}

		public override string ConvertToString(float obj)
		{
			return obj.ToString();
		}
	}

	/*[EditablePropertyFactory(typeof(BasicKeyframeList<float>))]
	public class FloatFactory : Factory
	{
		public GameObject GetEditor(object property)
		{
			EditableProperty<float> editableProperty = new EditableProperty<float>((BasicKeyframeList<float>)property, null);
			GameObject editorObject = new GameObject();
			Float editor = editorObject.AddComponent<Float>();
			editor.Property = editableProperty;
			return editorObject;
		}
	}*/
}
