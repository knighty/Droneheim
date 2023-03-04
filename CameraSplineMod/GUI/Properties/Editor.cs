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

			GameObject root = componentInitialiser.Layout(false);
			root.GetComponent<VerticalLayoutGroup>().childControlHeight = false;

			//root.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.MinSize;

			string name = "Property";
			foreach(EditablePropertyModifierAttribute modifier in modifiers)
			{
				if (modifier.GetType() == typeof(EditablePropertyAttribute))
				{
					name = ((EditablePropertyAttribute)modifier).Name;
				}
			}
			GameObject textName = componentInitialiser.Text(name, root, "Window.Title");

			componentInitialiser.Button("Test Button", root, "Button");

			/*GameObject editorUi = CreateEditorUI(prop);
			editorUi.transform.SetParent(root.transform);

			GameObject keyframeUI = CreateKeyframeUI<T>(prop);
			keyframeUI.transform.SetParent(root.transform);*/

			return root;
		}

		protected abstract GameObject CreateEditorUI(KeyframedEditableProperty<T> property);

		protected GameObject CreateKeyframeUI<T>(KeyframedEditableProperty<T> editableProperty)
		{
			GameObject ui = new GameObject();
			ui.AddComponent<Text>().text = "KF";
			return ui;
		}
	}

	[EditablePropertyFactory(typeof(BasicKeyframeList<float>))]
	class FloatEditablePropertyFactory : SimpleEditablePropertyFactory<float>
	{
		protected override GameObject CreateEditorUI(KeyframedEditableProperty<float> property)
		{
			GameObject editorObject = ComponentInitialiser.Panel();
			//Float editor = editorObject.AddComponent<Float>();
			editorObject.GetComponent<Image>().color = Color.red;
			editorObject.GetComponent<RectTransform>().sizeDelta = new Vector2(50, 50);
			//editorObject.GetComponent<RectTransform>().sizeDelta = new Vector2(100, 100);
			//editor.Property = property;
			return editorObject;
		}
	}

	[EditablePropertyFactory(typeof(BasicKeyframeList<SplineNode>))]
	class SplineNodeEditablePropertyFactory : SimpleEditablePropertyFactory<SplineNode>
	{
		protected override GameObject CreateEditorUI(KeyframedEditableProperty<SplineNode> property)
		{
			GameObject editorObject = new GameObject();
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
			GetComponent<VerticalLayoutGroup>().childControlHeight = false;
		}

		/*public KeyframeablePropertyEditor() : base()
		{
			editablePropertyFactories[typeof(string)] = CreateEditableProperty<string>;
			editablePropertyFactories[typeof(float)] = CreateEditableProperty<float>;
			editablePropertyFactories[typeof(Vector2)] = CreateEditableProperty<Vector2>;
			editablePropertyFactories[typeof(Vector3)] = CreateEditableProperty<Vector3>;
			editablePropertyFactories[typeof(SplineNode)] = CreateEditableProperty<SplineNode>;

			RegisterEditor<float>(new Editors.FloatFactory());
			RegisterEditor<SplineNode>(new Editors.SplineNodeFactory());
		}*/

		/*public IEditableProperty CreateEditableProperty<T>(object obj)
		{
			EditableProperty<T> prop = new EditableProperty<T>((BasicKeyframeList<T>)obj, null);
			return prop;
		}*/

		/*public void RegisterEditor<T>(Editors.Factory factory)
		{
			factories[typeof(T)] = factory;
		}

		public Factory GetFactory(Type t)
		{
			return (Factory)factories[t];
		}*/

		/*protected GameObject CreatePropertyEditorUI<T>(IEditableProperty property)
		{
			GameObject root = componentInitialiser.Layout(true);

			GameObject textName = componentInitialiser.Text(property.Name, root);

			GameObject editor = GetFactory(property.Type).GetEditor(property);
			editor.transform.SetParent(root.transform);

			return root;
		}*/

		/*public void AddProperties<T>(List<EditableProperty<T>> properties)
		{
			foreach (EditableProperty<T> property in properties)
			{
				GameObject editor = CreatePropertyEditorUI<T>(property);
				editor.transform.SetParent(transform);
			}
		}*/

		public void AddProperties(object obj, Timeline timeline)
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
					if (a != null && a.Type == prop.PropertyType)
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