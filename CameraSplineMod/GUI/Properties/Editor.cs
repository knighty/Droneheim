using Droneheim.GUI.Properties.Editors;
using Droneheim.Spline;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using static CharacterDrop;

namespace Droneheim.GUI.Properties
{
	abstract class EditablePropertyFactory
	{
		public abstract GameObject CreateEditor(object obj, IEnumerable<EditablePropertyModifierAttribute> modifiers, ComponentInitialiser componentInitialiser, Timeline timeline);
	}

	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
	public class EditablePropertyFactoryAttribute : Attribute
	{
		public Type Type { get; set; }
		public EditablePropertyFactoryAttribute(Type type)
		{
			Type = type;
		}
	}

	abstract class SimpleEditablePropertyFactory<T> : EditablePropertyFactory
	{
		public override GameObject CreateEditor(object obj, IEnumerable<EditablePropertyModifierAttribute> modifiers, ComponentInitialiser componentInitialiser, Timeline timeline)
		{
			if (timeline == null)
			{
				throw new ArgumentNullException(nameof(timeline));
			}
			if (componentInitialiser == null)
			{
				throw new ArgumentNullException(nameof(componentInitialiser));
			}
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

	[EditablePropertyFactory(typeof(BasicKeyframeList<float>))]
	class FloatEditablePropertyFactory : SimpleEditablePropertyFactory<float>
	{
		protected override GameObject CreateEditorUI(KeyframedEditableProperty<float> property)
		{
			GameObject editorObject = new GameObject();
			editorObject.AddComponent<HorizontalLayoutGroup>();
			editorObject.GetComponent<HorizontalLayoutGroup>().childControlHeight = true;

			LayoutElement layoutElement = editorObject.AddComponent<LayoutElement>();
			layoutElement.flexibleWidth = 0;
			layoutElement.minWidth = 100;

			ComponentInitialiser.InitAnchors(editorObject.GetComponent<RectTransform>());

			Float editor = editorObject.AddComponent<Float>();
			editor.Property = property;
			return editorObject;
		}
	}

	[EditablePropertyFactory(typeof(BasicKeyframeList<SplineNode>))]
	class SplineNodeEditablePropertyFactory : SimpleEditablePropertyFactory<SplineNode>
	{
		protected override GameObject CreateEditorUI(KeyframedEditableProperty<SplineNode> property)
		{
			GameObject editorObject = new GameObject();
			editorObject.AddComponent<HorizontalLayoutGroup>();
			editorObject.GetComponent<HorizontalLayoutGroup>().childControlHeight = true;

			LayoutElement layoutElement = editorObject.AddComponent<LayoutElement>();
			layoutElement.flexibleWidth = 0;
			layoutElement.minWidth = 100;
			SplineNodeEditor editor = editorObject.AddComponent<SplineNodeEditor>();
			editor.Property = property;
			return editorObject;
		}
	}

	[RequireComponent(typeof(VerticalLayoutGroup))]
	public class KeyframeablePropertyEditor : MonoBehaviour
	{
		protected delegate EditableProperty EditablePropertyCreateFunction(object property);

		public event Action OnEdit;
		public event Action OnKeyframe;

		//protected Dictionary<Type, object> factories = new Dictionary<Type, object>();
		//protected Dictionary<Type, EditablePropertyCreateFunction> editablePropertyFactories = new Dictionary<Type, EditablePropertyCreateFunction>();
		public ComponentInitialiser componentInitialiser;

		public void Start()
		{
			ComponentInitialiser.InitAnchors(GetComponent<RectTransform>());
			ComponentInitialiser.InitContentFitter(gameObject.AddComponent<ContentSizeFitter>());
			GetComponent<VerticalLayoutGroup>().childControlHeight = true;
			GetComponent<VerticalLayoutGroup>().spacing = 10;
		}

		public void SetObject(object obj, Timeline timeline)
		{
			if (timeline == null)
			{
				throw new ArgumentNullException(nameof(timeline));
			}
			PropertyInfo[] objProperties = obj.GetType().GetProperties();
			foreach (PropertyInfo prop in objProperties)
			{
				EditablePropertyAttribute epa = prop.GetCustomAttribute<EditablePropertyAttribute>();
				if (epa == null)
					continue;

				// Iterate over all types and find one that can create an editor for this type
				foreach (Type t in Assembly.GetExecutingAssembly().GetTypes())
				{
					EditablePropertyFactoryAttribute a = t.GetCustomAttribute<EditablePropertyFactoryAttribute>();
					if (a != null && (prop.PropertyType.IsSubclassOf(a.Type) || prop.PropertyType == a.Type))
					{
						EditablePropertyFactory factory = (EditablePropertyFactory)Activator.CreateInstance(t);
						GameObject editorObject = factory.CreateEditor(prop.GetValue(obj), prop.GetCustomAttributes<EditablePropertyModifierAttribute>(), componentInitialiser, timeline);
						editorObject.transform.SetParent(gameObject.transform);
					}
				}
			}
		}
	}
}