using Droneheim.Spline;
using UnityEngine;
using UnityEngine.UI;

using System;

namespace Droneheim.GUI.Properties.Editors
{
	[EditablePropertyEditor(typeof(SplineKeyframes), typeof(KeyframeEditorFactory))]
	public class KeyframedSplineEditor : KeyframedPropertyEditor<SplineNode, SplineKeyframes>
	{
		InputField[] inputFields = new InputField[6];
		SplineNode currentValue = SplineNode.Identity;

		protected override SplineNode Value
		{
			set
			{
				currentValue = value;

				inputFields[0].text = string.Format("{0:F1}", currentValue.Position.x);
				inputFields[1].text = string.Format("{0:F1}", currentValue.Position.y);
				inputFields[2].text = string.Format("{0:F1}", currentValue.Position.z);

				Vector3 euler = currentValue.Rotation.eulerAngles;
				inputFields[3].text = string.Format("{0:F0}", euler.x);
				inputFields[4].text = string.Format("{0:F0}", euler.y);
				inputFields[5].text = string.Format("{0:F0}", euler.z);
			}
			get
			{
				return currentValue;
			}
		}

		protected override void CreateUI()
		{
			VerticalLayoutGroup layout = gameObject.AddComponent<VerticalLayoutGroup>();
			gameObject.AddComponent<StyledElement>().SetTypeClasses("Element", "property");

			layout.childControlWidth = true;
			layout.childControlHeight = true;
			layout.spacing = 8;
			layout.childForceExpandWidth = false;
			layout.childForceExpandHeight = false;
			layout.childScaleHeight = false;

			GameObject top = CreateTopUI();
			top.transform.SetParent(transform, false);

			GameObject translationUI = CreateTranslationUI();
			translationUI.transform.SetParent(transform, false);

			GameObject rotationUI = CreateRotationUI();
			rotationUI.transform.SetParent(transform, false);
		}

		protected GameObject CreateTopUI()
		{
			GameObject top = ComponentInitialiser.Layout(true);
			top.AddComponent<StyledElement>().SetTypeClasses("Element", "property");

			HorizontalLayoutGroup layoutGroup = top.GetComponent<HorizontalLayoutGroup>();
			layoutGroup.childControlWidth = true;
			layoutGroup.childControlHeight = true;
			layoutGroup.spacing = 10;
			layoutGroup.childForceExpandWidth = false;
			layoutGroup.childForceExpandHeight = false;
			layoutGroup.childScaleHeight = false;

			GameObject textName = CreatePropertyNameUI();
			textName.transform.SetParent(top.transform, false);
			propertyNameComponent = textName.GetComponent<Text>();

			GameObject updateButton = ComponentInitialiser.Button("Camera Position", top);
			updateButton.GetComponent<Button>().onClick.AddListener(() =>
			{
				Value = new SplineNode(Camera.main.transform);
			});

			GameObject keyframeUI = CreateKeyframeUI();
			keyframeUI.transform.SetParent(top.transform, false);
			keyframeComponent = keyframeUI.GetComponent<KeyframeUI>();

			return top;
		}

		protected GameObject CreateTranslationUI()
		{
			GameObject layout = ComponentInitialiser.Layout();
			layout.GetComponent<HorizontalOrVerticalLayoutGroup>().spacing = 8;

			GameObject positionLayout = ComponentInitialiser.Layout(true);
			positionLayout.GetComponent<HorizontalOrVerticalLayoutGroup>().spacing = 8;
			positionLayout.transform.SetParent(layout.transform, false);

			GameObject textName = ComponentInitialiser.Text("", positionLayout, "name");
			textName.AddComponent<LayoutElement>().flexibleWidth = 1;

			void InitField(int i, string text)
			{
				GameObject label = ComponentInitialiser.Text(text);
				label.transform.SetParent(positionLayout.transform, false);

				GameObject input = ComponentInitialiser.TextInput();
				input.transform.SetParent(positionLayout.transform, false);
				input.AddComponent<LayoutElement>().minWidth = 60;

				inputFields[i] = input.GetComponent<InputField>();
			}

			InitField(0, "x");
			InitField(1, "y");
			InitField(2, "z");

			return layout;
		}

		protected GameObject CreateRotationUI()
		{
			GameObject layout = ComponentInitialiser.Layout();
			layout.GetComponent<HorizontalOrVerticalLayoutGroup>().spacing = 8;

			GameObject positionLayout = ComponentInitialiser.Layout(true);
			positionLayout.GetComponent<HorizontalOrVerticalLayoutGroup>().spacing = 8;
			positionLayout.transform.SetParent(layout.transform, false);

			GameObject textName = ComponentInitialiser.Text("", positionLayout, "name");
			textName.AddComponent<LayoutElement>().flexibleWidth = 1;

			void InitField(int i, string text)
			{
				GameObject label = ComponentInitialiser.Text(text);
				label.transform.SetParent(positionLayout.transform, false);

				GameObject input = ComponentInitialiser.TextInput();
				input.transform.SetParent(positionLayout.transform, false);
				input.AddComponent<LayoutElement>().minWidth = 60;

				inputFields[i] = input.GetComponent<InputField>();
			}

			InitField(3, "Pitch");
			InitField(4, "Yaw");
			InitField(5, "Roll");

			return layout;
		}

		protected override GameObject CreateEditorUI()
		{
			throw new System.NotImplementedException();
		}
	}
}
