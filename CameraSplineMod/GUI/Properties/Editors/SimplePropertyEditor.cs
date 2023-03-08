using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;

namespace Droneheim.GUI.Properties.Editors
{
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
				input.transform.SetParent(gameObject.transform, false);

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

	abstract class SimpleEditablePropertyFactory<T> : EditablePropertyFactory
	{
		public override GameObject CreateEditor(object obj, IEnumerable<EditablePropertyModifierAttribute> modifiers, ComponentInitialiser componentInitialiser, Timeline timeline)
		{
			Assert.IsNotNull(timeline);
			Assert.IsNotNull(componentInitialiser);

			KeyframedEditableProperty<T> prop = new KeyframedEditableProperty<T>((BasicKeyframeList<T>)obj, timeline.PlaybackController);

			GameObject root = ComponentInitialiser.Layout(true, "property");
			//GameObject root = new GameObject();
			root.GetComponent<HorizontalLayoutGroup>().childControlWidth = true;
			root.GetComponent<HorizontalLayoutGroup>().childControlHeight = true;
			//root.GetComponent<HorizontalLayoutGroup>().childForceExpandHeight = true;
			root.GetComponent<HorizontalLayoutGroup>().spacing = 10;

			string name = "Property";
			foreach (EditablePropertyModifierAttribute modifier in modifiers)
			{
				if (modifier.GetType() == typeof(EditablePropertyAttribute))
				{
					name = ((EditablePropertyAttribute)modifier).Name;
				}
			}

			{
				GameObject textName = componentInitialiser.Text(name, root, "name");
				//textName.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 100);
				LayoutElement layoutElement = textName.AddComponent<LayoutElement>();
				layoutElement.flexibleWidth = 1;
				//layoutElement.minHeight = 100;
			}

			GameObject editorUi = CreateEditorUI(prop);
			editorUi.transform.SetParent(root.transform);

			{
				GameObject keyframeUI = CreateKeyframeUI<T>(prop);
				keyframeUI.transform.SetParent(root.transform, false);
				LayoutElement layoutElement = keyframeUI.AddComponent<LayoutElement>();
				layoutElement.flexibleWidth = 0;
				//layoutElement.minWidth = 100;
			}


			return root;
		}

		protected abstract GameObject CreateEditorUI(KeyframedEditableProperty<T> property);

		protected GameObject CreateKeyframeUI<T>(KeyframedEditableProperty<T> editableProperty)
		{
			GameObject container = ComponentInitialiser.Layout(true);

			GameObject InitButton(Sprite sprite)
			{
				GameObject button = ComponentInitialiser.Image(sprite);
				button.transform.SetParent(container.transform, false);

				return button;
			}

			GameObject buttonPrevious = InitButton(DroneheimResources.PreviousKeyframe);
			GameObject buttonKeyframe = InitButton(DroneheimResources.KeyframeOff);
			GameObject buttonNext = InitButton(DroneheimResources.NextKeyframe);

			return container;
		}
	}
}