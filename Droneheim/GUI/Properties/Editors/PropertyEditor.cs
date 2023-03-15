using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using System.Reflection;
using System;
using Droneheim.Commands;

namespace Droneheim.GUI.Properties.Editors
{
	abstract public class BasePropertyEditor : MonoBehaviour
	{
		internal abstract object ObjectProperty { set; }
	}

	abstract public class PropertyEditor<T> : BasePropertyEditor
	{
		internal List<EditablePropertyModifierAttribute> Modifiers;
		public abstract EditableProperty<T> Property { set; }

		internal override object ObjectProperty
		{
			set
			{
				Property = (EditableProperty<T>)value;
			}
		}
	}

	public abstract class SimplePropertyEditor<T> : PropertyEditor<T>
	{
		protected EditableProperty<T> property;
		public override EditableProperty<T> Property { set { property = value as EditableProperty<T>; } }

		protected Text propertyNameComponent;

		public void Awake()
		{
			CreateUI();
		}

		protected virtual void CreateUI()
		{
			gameObject.AddComponent<HorizontalLayoutGroup>();

			gameObject.AddComponent<StyledElement>().SetTypeClasses("Element", "property");
			GetComponent<HorizontalLayoutGroup>().childControlWidth = true;
			GetComponent<HorizontalLayoutGroup>().childControlHeight = true;
			GetComponent<HorizontalLayoutGroup>().spacing = 10;
			GetComponent<HorizontalLayoutGroup>().childForceExpandWidth = false;
			GetComponent<HorizontalLayoutGroup>().childForceExpandHeight = false;
			GetComponent<HorizontalLayoutGroup>().childScaleHeight = false;

			GameObject textName = CreatePropertyNameUI();
			textName.transform.SetParent(gameObject.transform, false);
			propertyNameComponent = textName.GetComponent<Text>();

			GameObject editorUi = CreateEditorUI();
			editorUi.transform.SetParent(transform);
		}

		public void Start()
		{
			Assert.IsNotNull(property, "Must set a property for editing");
			propertyNameComponent.text = property.GetAttribute<EditablePropertyAttribute>()?.Name ?? "Property";
		}

		protected GameObject CreatePropertyNameUI()
		{
			GameObject textName = ComponentInitialiser.Text("", gameObject, "name");
			textName.AddComponent<LayoutElement>().flexibleWidth = 1;
			return textName;
		}

		protected abstract GameObject CreateEditorUI();

		protected void UpdateValue()
		{
			CommandList.Instance.Add(new SetEditableProperty<T>(property, Value));
		}

		protected abstract T Value { set; get; }
	}

	public abstract class KeyframedPropertyEditor<T, KT> : PropertyEditor<KT> where KT : BasicKeyframeList<T>
	{
		protected KeyframeableEditableProperty<KT> property;
		public override EditableProperty<KT> Property { set { property = value as KeyframeableEditableProperty<KT>; } }

		protected KeyframeUI keyframeComponent;
		protected Text propertyNameComponent;

		public void Awake()
		{
			CreateUI();
		}

		protected virtual void CreateUI()
		{
			gameObject.AddComponent<HorizontalLayoutGroup>();

			gameObject.AddComponent<StyledElement>().SetTypeClasses("Element", "property");
			HorizontalLayoutGroup layoutGroup = GetComponent<HorizontalLayoutGroup>();
			layoutGroup.childControlWidth = true;
			layoutGroup.childControlHeight = true;
			layoutGroup.childForceExpandWidth = false;
			layoutGroup.childForceExpandHeight = false;
			layoutGroup.spacing = 10;

			GameObject textName = CreatePropertyNameUI();
			textName.transform.SetParent(gameObject.transform, false);
			propertyNameComponent = textName.GetComponent<Text>();

			GameObject editorUi = CreateEditorUI();
			editorUi.transform.SetParent(transform);

			GameObject keyframeUI = CreateKeyframeUI();
			keyframeUI.transform.SetParent(transform, false);
			keyframeComponent = keyframeUI.GetComponent<KeyframeUI>();
		}

		public void Start()
		{
			Assert.IsNotNull(property, "Must set a property for editing");

			propertyNameComponent.text = property.GetAttribute<EditablePropertyAttribute>()?.Name ?? "Property";

			property.Timeline.OnFrameChanged += OnFrameChanged;
			OnFrameChanged(property.Timeline.CurrentFrame);
		}

		protected void OnFrameChanged(int frame)
		{
			keyframeComponent.UpdateKeyframed(property.Value.HasKeyframeAt(frame));
			Value = property.Value.GetValueAt(frame);
		}

		protected GameObject CreatePropertyNameUI()
		{
			GameObject textName = ComponentInitialiser.Text("", gameObject, "name");
			textName.AddComponent<LayoutElement>().flexibleWidth = 1;
			return textName;
		}

		protected abstract GameObject CreateEditorUI();

		protected GameObject CreateKeyframeUI()
		{
			GameObject container = new GameObject();
			container.AddComponent<LayoutElement>().flexibleWidth = 0;
			KeyframeUI keyframeComponent = container.AddComponent<KeyframeUI>();
			keyframeComponent.OnKeyframePress += ToggleKeyframe;
			keyframeComponent.OnPreviousKeyframePress += () => property.Timeline.GoTo(property.Value.GetPreviousKeyframe(property.Timeline.CurrentFrame));
			keyframeComponent.OnNextKeyframePress += () => property.Timeline.GoTo(property.Value.GetNextKeyframe(property.Timeline.CurrentFrame));

			return container;
		}

		protected void ToggleKeyframe()
		{
			int frame = property.Timeline.CurrentFrame;
			if (property.Value.HasKeyframeAt(frame))
				RemoveKeyframe();
			else
				SetKeyframe();
		}

		protected void RemoveKeyframe()
		{
			int frame = property.Timeline.CurrentFrame;
			CommandList.Instance.Add(new RemoveKeyframe<T>(property.Value, frame));
			//property.Value.RemoveKeyframe(frame);
			OnFrameChanged(frame);
		}

		protected void SetKeyframe()
		{
			int frame = property.Timeline.CurrentFrame;
			CommandList.Instance.Add(new SetKeyframe<T>(property.Value, frame, Value));
			//property.Value.SetKeyframe(frame, Value);
			OnFrameChanged(frame);
		}

		protected abstract T Value { set; get; }
	}

	[RequireComponent(typeof(HorizontalLayoutGroup))]
	public class KeyframeUI : MonoBehaviour
	{
		public Button ButtonPrevious;
		public Button ButtonKeyframe;
		public Button ButtonNext;

		public Action OnKeyframePress;
		public Action OnPreviousKeyframePress;
		public Action OnNextKeyframePress;

		public void Awake()
		{
			GetComponent<HorizontalLayoutGroup>().childForceExpandHeight = false;
			GetComponent<HorizontalLayoutGroup>().spacing = 4;
			gameObject.AddComponent<LayoutElement>().flexibleHeight = 1;

			gameObject.AddComponent<StyledElement>().SetTypeClasses("Element");

			Button InitButton(Sprite sprite)
			{
				GameObject button = ComponentInitialiser.Image(sprite);
				button.transform.SetParent(gameObject.transform);

				LayoutElement layoutElement = button.GetComponent<LayoutElement>();
				layoutElement.minHeight = 16;
				layoutElement.minWidth = 16;
				layoutElement.preferredHeight = 16;
				layoutElement.preferredWidth = 16;
				layoutElement.layoutPriority = 2;

				button.AddComponent<Button>();

				return button.GetComponent<Button>();
			}

			ButtonPrevious = InitButton(DroneheimResources.PreviousKeyframe);
			ButtonPrevious.GetComponent<Image>().color = new Color32(80, 80, 80, 255);
			ButtonKeyframe = InitButton(DroneheimResources.KeyframeOff);
			ButtonNext = InitButton(DroneheimResources.NextKeyframe);
			ButtonNext.GetComponent<Image>().color = new Color32(80, 80, 80, 255);

			ButtonPrevious.onClick.AddListener(() => OnPreviousKeyframePress?.Invoke());
			ButtonKeyframe.onClick.AddListener(() => OnKeyframePress?.Invoke());
			ButtonNext.onClick.AddListener(() => OnNextKeyframePress?.Invoke());
		}

		public void UpdateKeyframed(bool keyframed)
		{
			ButtonKeyframe.GetComponent<Image>().sprite = keyframed ? DroneheimResources.KeyframeOn : DroneheimResources.KeyframeOff;
		}
	}
}