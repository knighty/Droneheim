using BepInEx.Configuration;
using Droneheim.GUI;
using Droneheim.GUI.Properties;
using Droneheim.Spline;
using System;
using UnityEngine;
using UnityEngine.UI;
using P = Droneheim.GUI.StyleProperties;

namespace Droneheim
{
	public class TestComp : Component
	{

	}

	[RequireComponent(typeof(TestComp))]
	[RequireComponent(typeof(SplineRenderer), typeof(Timeline))]
	public class Droneheim : MonoBehaviour
	{
		public Timeline Timeline { get { return GetComponent<Timeline>(); } }
		public CameraProperties CameraProperties { get; set; } = new CameraProperties();

		private CameraShakeController cameraShakeController = new CameraShakeController();

		protected int positionKeyframeNext = 0;

		private KeyboardShortcut clearSpline = new KeyboardShortcut(UnityEngine.KeyCode.I);
		private KeyboardShortcut toggleFollowSpline = new KeyboardShortcut(UnityEngine.KeyCode.Y);
		private KeyboardShortcut addPositionKeyframe = new KeyboardShortcut(UnityEngine.KeyCode.U);

		protected void InitialiseToolbar(GameObject parent, ComponentInitialiser componentInitialiser)
		{
			GameObject obj = ComponentInitialiser.Panel("Window");
			obj.AddComponent<LayoutElement>().flexibleHeight = 0;
			obj.transform.SetParent(parent.transform);
		}

		protected void InitialiseCentre(GameObject parent, ComponentInitialiser componentInitialiser)
		{
			GameObject obj = new GameObject();
			obj.AddComponent<RectTransform>();
			obj.GetComponent<RectTransform>().pivot = Vector2.zero;

			/*HorizontalLayoutGroup horizontalLayoutGroup = obj.AddComponent<HorizontalLayoutGroup>();
			obj.AddComponent<ContentSizeFitter>();
			horizontalLayoutGroup.childControlHeight = true;
			horizontalLayoutGroup.childForceExpandHeight = false;*/

			obj.AddComponent<LayoutElement>().flexibleHeight = 1;
			obj.transform.SetParent(parent.transform);

			Window window = new Window(componentInitialiser, "Example Window")
			{
				AnchorMin = new Vector2(1, 0),
				AnchorMax = new Vector2(1, 0.5f),
				//Size = new Vector2(500, 0),
			};
			window.PositionRect.offsetMin = new Vector2(-300, 0);
			window.AddTo(obj);

			/*GameObject propertyEditor = new GameObject();
			KeyframeablePropertyEditor editorComponent = propertyEditor.AddComponent<KeyframeablePropertyEditor>();
			editorComponent.componentInitialiser = componentInitialiser;
			editorComponent.AddProperties(CameraProperties, Timeline);
			propertyEditor.transform.SetParent(window.Content.transform);*/
		}

		protected void InitialiseTimeline(GameObject parent, ComponentInitialiser componentInitialiser)
		{
			GameObject obj = ComponentInitialiser.Panel("Window");
			LayoutElement layoutElement = obj.AddComponent<LayoutElement>();
			layoutElement.flexibleHeight = 0;
			layoutElement.preferredHeight = 200;
			obj.transform.SetParent(parent.transform);
		}

		static Color RGBColor(int r, int g, int b)
		{
			return new Color((float)r / 255.0f, (float)g / 255.0f, (float)b / 255.0f);
		}

		public void InitialiseUI()
		{
			GameObject ui = new GameObject("Canvas");
			try
			{
				Stylers stylers = new Stylers();
				ComponentInitialiser componentInitialiser = new ComponentInitialiser();

				Stylesheet stylesheet = ui.AddComponent<Stylesheet>();

				Texture2D tex = AssetUtils.LoadTexture("ui.png");
				int d = tex.height;
				Sprite spriteWindowBackground = Sprite.Create(tex, new Rect(d * 0, 0, d, d), new Vector2(0.5f, 0.5f), 1, 0, SpriteMeshType.FullRect, new Vector4(16, 16, 16, 16));
				Sprite spriteInsetBackground = Sprite.Create(tex, new Rect(d * 1, 0, d, d), new Vector2(0.5f, 0.5f), 1, 0, SpriteMeshType.FullRect, new Vector4(16, 16, 16, 16));
				Sprite spriteConfirmBackground = Sprite.Create(tex, new Rect(d * 2, 0, d, d), new Vector2(0.5f, 0.5f), 1, 0, SpriteMeshType.FullRect, new Vector4(16, 16, 16, 16));


				stylesheet.AddStyle("Body", s =>
				{
					s.Set(P.Font, Resources.GetBuiltinResource<Font>("Arial.ttf"));
				});

				stylesheet.AddStyle("Button", s =>
				{
					s.Set(P.BackgroundImage, spriteConfirmBackground);
					s.Set(P.Padding, new RectOffset(16, 16, 16, 16));
					s.Set(P.FontWeight, FontStyle.Bold);
					s.Set(P.Color, Color.white);
				});

				stylesheet.AddStyle("Input", s =>
				{
					s.Set(P.BackgroundImage, spriteInsetBackground);
					s.Set(P.BackgroundColor, RGBColor(26, 26, 26));
					s.Set(P.Padding, new RectOffset(8, 8, 8, 8));
					s.Set(P.FontWeight, FontStyle.Bold);
					s.Set(P.FontSize, 14);
					s.Set(P.Color, Color.white);
				});

				stylesheet.AddStyle(".Window", s =>
				{
					s.Set(P.Padding, new RectOffset(16, 16, 16, 16));
					s.Set(P.BackgroundImage, spriteWindowBackground);
					s.Set(P.Color, Color.white);
				});

				stylesheet.AddStyle(".Window .Title", s =>
				{
					s.Set(P.BackgroundImage, spriteInsetBackground);
					s.Set(P.BackgroundColor, RGBColor(87, 82, 76));
					s.Set(P.FontSize, 18);
					s.Set(P.FontWeight, FontStyle.Bold);
					s.Set(P.Color, Color.white);
					s.Set(P.Padding, new RectOffset(16, 16, 16, 16));
				});

				stylesheet.styleTree.Print(0);

				Canvas c = ui.AddComponent<Canvas>();
				c.renderMode = RenderMode.ScreenSpaceCamera;
				c.pixelPerfect = true;
				c.referencePixelsPerUnit = 1;

				ui.AddComponent<StyledElement>().ElementType = "Body";
				ui.AddComponent<CanvasScaler>();
				ui.AddComponent<GraphicRaycaster>();

				ui.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
				ui.GetComponent<RectTransform>().pivot = Vector2.zero;

				VerticalLayoutGroup mainLayoutGroup = ui.AddComponent<VerticalLayoutGroup>();
				ui.AddComponent<ContentSizeFitter>();
				mainLayoutGroup.childControlHeight = true;
				mainLayoutGroup.childForceExpandHeight = false;
				mainLayoutGroup.padding = new RectOffset(16, 16, 16, 16);
				mainLayoutGroup.spacing = 16;

				InitialiseToolbar(ui, componentInitialiser);
				InitialiseCentre(ui, componentInitialiser);
				InitialiseTimeline(ui, componentInitialiser);

				ui.transform.SetParent(gameObject.transform);
			}
			catch (Exception)
			{
				Destroy(ui);
				throw;
			}
		}

		public void Start()
		{
			if (GetComponent<SplineRenderer>() == null)
				gameObject.AddComponent<SplineRenderer>();
			if (GetComponent<Timeline>() == null)
				gameObject.AddComponent<Timeline>();
			/*SplineFollower splineFollower = GameCamera.instance.gameObject.AddComponent<SplineFollower>();
			splineFollower.Spline = Spline;
			splineFollower.Timeline = timeline;*/

			InitialiseUI();
		}

		protected void HandleInput()
		{

		}

		public void LateUpdate()
		{
			return;
			if (toggleFollowSpline.IsDown())
			{
				Debug.Log("Entering Follow Spline Mode");
				if (Timeline.PlaybackController.Playing)
				{
					Timeline.PlaybackController.Stop();
				}
				else
				{
					Timeline.PlaybackController.Play();
				}
			}

			if (Timeline.PlaybackController.Playing)
			{
				SplineNode transform = CameraProperties.Transform.GetValueAt(Timeline.PlaybackController.CurrentFrame);
				GameCamera.instance.gameObject.transform.position = transform.Position + cameraShakeController.GetTranslation(CameraProperties, Timeline.PlaybackController.CurrentTime);
				GameCamera.instance.gameObject.transform.rotation = transform.Rotation * cameraShakeController.GetRotation(CameraProperties, Timeline.PlaybackController.CurrentTime);
			}

			if (addPositionKeyframe.IsDown())
			{
				/*Spline.AddKeyframe(positionKeyframeNext, new SplineNode(GameCamera.instance.transform));
				Debug.Log("Added keyframe: " + positionKeyframeNext + ", Position: " + GameCamera.instance.transform.position);// + ", FOV: " + GameCamera.instance.m_fov);
				positionKeyframeNext = Spline.GetLastKeyframe() + 90;// + UnityEngine.Random.Range(-60,60);*/
			}

			if (clearSpline.IsDown())
			{
				positionKeyframeNext = 0;
				//KeyframeController.spline.Clear();
			}
		}
	}
}
