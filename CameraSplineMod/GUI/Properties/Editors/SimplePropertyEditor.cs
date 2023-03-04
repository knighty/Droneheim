using UnityEngine;
using UnityEngine.UI;

namespace Droneheim.GUI.Properties.Editors
{
	/*[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
	public sealed class EditablePropertyAttribute : Attribute
	{

	}*/

	[RequireComponent(typeof(RectTransform))]
	abstract public class SimplePropertyEditor<T> : MonoBehaviour
	{
		protected KeyframedEditableProperty<T> property;
		public KeyframedEditableProperty<T> Property
		{
			get => property;
			set
			{
				GameObject input = ComponentInitialiser.TextInput();
				input.transform.SetParent(gameObject.transform);

				property = value;
				OnPropertyChanged(null);
				property.OnChange += OnPropertyChanged;

				InputField.onEndEdit.AddListener(OnEditField);
			}
		}

		protected InputField inputField = null;
		protected InputField InputField
		{
			get => inputField = inputField ?? GetComponentInChildren<InputField>();
		}

		public abstract T ConvertFromString(string str);

		public abstract string ConvertToString(T obj);

		public SimplePropertyEditor() : base()
		{
			
		}

		public void OnPropertyChanged(object obj)
		{
			InputField.text = ConvertToString(property.Value);
		}

		public void OnEditField(string text)
		{
			property.Value = ConvertFromString(text);
		}

		public void OnDestroy()
		{
			property.OnChange -= OnPropertyChanged;
		}

		public void Start()
		{
			//RectTransform rect = GetComponent<RectTransform>();
		}
	}
}