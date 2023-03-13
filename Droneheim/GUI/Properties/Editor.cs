using Droneheim.GUI.Properties.Editors;
using Droneheim.Timeline;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace Droneheim.GUI.Properties
{
	public interface EditorFactory
	{
		GameObject TryHandle(Type editorType, PropertyInfo propertyInfo, object obj);
	}

	public class StandardEditorFactory : EditorFactory
	{
		public GameObject TryHandle(Type editorType, PropertyInfo propertyInfo, object obj)
		{
			GameObject editorObject = new GameObject();
			BasePropertyEditor editor = (BasePropertyEditor)editorObject.AddComponent(editorType);

			Type type = typeof(EditableProperty<>).MakeGenericType(propertyInfo.PropertyType);

			object o = Activator.CreateInstance(type, new object[] { propertyInfo, obj });
			editor.ObjectProperty = o;

			return editorObject;
		}
	}

	public class KeyframeEditorFactory : EditorFactory
	{
		protected BlockTimelineController timeline;

        public KeyframeEditorFactory(BlockTimelineController timeline)
        {
            this.timeline = timeline;
        }

        public GameObject TryHandle(Type editorType, PropertyInfo propertyInfo, object obj)
		{
			/*bool valid =
				propertyInfo.PropertyType.IsGenericType && (
					propertyInfo.PropertyType.GetGenericTypeDefinition().IsSubclassOf(typeof(BasicKeyframeList<>)) ||
					propertyInfo.PropertyType.GetGenericTypeDefinition() == (typeof(BasicKeyframeList<>))
				);
			if (!valid)
				return null;*/

			GameObject editorObject = new GameObject();
			BasePropertyEditor editor = (BasePropertyEditor)editorObject.AddComponent(editorType);

			Type type = typeof(KeyframeableEditableProperty<>).MakeGenericType(propertyInfo.PropertyType);

			object o = Activator.CreateInstance(type, new object[] { propertyInfo, obj, timeline });
			editor.ObjectProperty = o;

			return editorObject;
		}
	}

	[RequireComponent(typeof(VerticalLayoutGroup))]
	public class PropertyEditorList : MonoBehaviour
	{
		public event Action OnEdit;
		public event Action OnKeyframe;

		public ComponentInitialiser componentInitialiser;

		protected List<EditorFactory> editorFactories = new List<EditorFactory>();

		public void Start()
		{
			ComponentInitialiser.InitAnchors(GetComponent<RectTransform>());
			ComponentInitialiser.InitContentFitter(gameObject.AddComponent<ContentSizeFitter>());
			GetComponent<VerticalLayoutGroup>().childControlHeight = true;
			GetComponent<VerticalLayoutGroup>().spacing = 10;
			gameObject.AddComponent<StyledElement>().SetTypeClasses("Element", "properties");
		}

		public void AddFactory(EditorFactory factory)
		{
			editorFactories.Add(factory);
		}

		public void SetObject(object obj, TimelineController timeline)
		{
			Assert.IsNotNull(timeline);

			PropertyInfo[] objProperties = obj.GetType().GetProperties();
			foreach (PropertyInfo prop in objProperties)
			{
				EditablePropertyAttribute epa = prop.GetCustomAttribute<EditablePropertyAttribute>();
				if (epa == null)
					continue;

				// Iterate over all types and find one that can create an editor for this type
				foreach (Type t in Assembly.GetExecutingAssembly().GetTypes())
				{
					EditablePropertyEditorAttribute a = t.GetCustomAttribute<EditablePropertyEditorAttribute>();
					if (a != null && (prop.PropertyType.IsSubclassOf(a.Type) || prop.PropertyType == a.Type))
					{
						foreach(var factory in editorFactories)
						{
							if (factory.GetType() == a.FactoryType || a.FactoryType == null)
							{
								GameObject editor = factory.TryHandle(t, prop, obj);
								if (editor != null)
								{
									editor.transform.SetParent(gameObject.transform);
									break;
								}
							}
						}
						/*GameObject editorObject = new GameObject();
						editorObject.transform.SetParent(gameObject.transform);
						BasePropertyEditor editor = (BasePropertyEditor)editorObject.AddComponent(t);

						Type type = typeof(KeyframeableEditableProperty<>).MakeGenericType(prop.PropertyType);

						object o = Activator.CreateInstance(type, new object[] { prop, obj, timeline });
						editor.ObjectProperty = o;*/
					}
				}
			}
		}
	}
}