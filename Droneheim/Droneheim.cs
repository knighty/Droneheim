using BepInEx.Configuration;
using Droneheim.GUI;
using Droneheim.GUI.Properties;
using Droneheim.Spline;
using Droneheim.Timeline;
using HarmonyLib;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using P = Droneheim.GUI.StyleProperties;

namespace Droneheim
{
	class DroneheimResources
	{
		public static Sprite WindowBackground;
		public static Sprite PaneBackground;
		public static Sprite InsetBackground;
		public static Sprite ConfirmBackground;
		public static Sprite InputBackground;
		public static Sprite KeyframeBlockBackground;

		public static Sprite PreviousKeyframe;
		public static Sprite NextKeyframe;

		public static Sprite KeyframeOff;
		public static Sprite KeyframeOn;

		public static Sprite Checkmark;

		public static Sprite TimelineScrubber;

		public static Sprite Expand;
		public static Sprite Collapse;

		public static Sprite Play;
		public static Sprite Pause;

		public static Sprite Next;
		public static Sprite Previous;

		public static Sprite Stop;

		public static Sprite Eye;
		public static Sprite EyeSlash;

		public static void Init()
		{
			Texture2D tex = AssetUtils.LoadTexture("ui.png");
			Texture2D tex2 = AssetUtils.LoadTexture("ui2.png");

			Sprite GetSprite(int x, int y, int size, int slicedBorder = 0)
			{
				if (slicedBorder > 0)
				{
					return Sprite.Create(tex, new Rect(x, y, size, size), new Vector2(0.5f, 0.5f), 1, 0, SpriteMeshType.FullRect, new Vector4(slicedBorder, slicedBorder, slicedBorder, slicedBorder));
				}
				return Sprite.Create(tex, new Rect(x, y, size, size), new Vector2(0.5f, 0.5f), 1, 0, SpriteMeshType.FullRect);
			}

			Sprite GetSprite2(int x, int y, int size, Vector4 slicedBorder)
			{
				if (slicedBorder.magnitude > 0)
				{
					return Sprite.Create(tex2, new Rect(x, y, size, size), new Vector2(0.5f, 0.5f), 1, 0, SpriteMeshType.FullRect, slicedBorder);
				}
				return Sprite.Create(tex2, new Rect(x, y, size, size), new Vector2(0.5f, 0.5f), 1, 0, SpriteMeshType.FullRect);
			}

			int full = 64;
			int half = 32;

			PaneBackground = GetSprite(full * 0, 0, full, 16);
			WindowBackground = GetSprite2(0, 128 * 3, 128, new Vector4(24, 8, 24, 64));
			InsetBackground = GetSprite(full * 1, 0, full, 16);
			ConfirmBackground = GetSprite(full * 2, 0, full, 16);
			InputBackground = GetSprite(full * 4, 0, full, 16);
			KeyframeBlockBackground = GetSprite(full * 8, 0, full, 31);

			KeyframeOff = GetSprite(full * 3, half, half);
			KeyframeOn = GetSprite(full * 3, 0, half);

			PreviousKeyframe = GetSprite(full * 3 + half * 1, 0, half);
			NextKeyframe = GetSprite(full * 3 + half * 1, half, half);

			Collapse = GetSprite(full * 5 + half, 0, half);
			Expand = GetSprite(full * 5 + half, half, half);

			Play = GetSprite(full * 6, half, half);
			Pause = GetSprite(full * 6, 0, half);

			Next = GetSprite(full * 6 + half, half, half);
			Previous = GetSprite(full * 6 + half, 0, half);

			Stop = GetSprite(full * 7, half, half);
			//Previous = GetSprite(full * 7, 0, half);

			Eye = GetSprite(full * 7 + half, half, half);
			EyeSlash = GetSprite(full * 7 + half, 0, half);

			Checkmark = Resources.FindObjectsOfTypeAll<Sprite>().First(f => f.name == "Checkmark");

			TimelineScrubber = GetSprite(full * 5, half, half);
		}
	}

	[RequireComponent(typeof(SplineRenderer), typeof(TimelineController))]
	public class Droneheim : MonoBehaviour
	{
		public TimelineController Timeline { get { return GetComponent<TimelineController>(); } }
		public CameraProperties CameraProperties { get; set; } = new CameraProperties();

		private CameraShakeController cameraShakeController = new CameraShakeController();

		protected int positionKeyframeNext = 0;

		private KeyboardShortcut clearSpline = new KeyboardShortcut(UnityEngine.KeyCode.I);
		private KeyboardShortcut toggleFollowSpline = new KeyboardShortcut(UnityEngine.KeyCode.Y);
		private KeyboardShortcut addPositionKeyframe = new KeyboardShortcut(UnityEngine.KeyCode.U);

		protected Text frameText;

		protected void InitialiseToolbar(GameObject parent, ComponentInitialiser componentInitialiser)
		{
			GameObject obj = ComponentInitialiser.Panel("Pane");
			obj.AddComponent<LayoutElement>().flexibleHeight = 0;
			obj.transform.SetParent(parent.transform);
		}

		protected void InitialiseCentre(GameObject parent, ComponentInitialiser componentInitialiser)
		{
			GameObject obj = new GameObject();
			ComponentInitialiser.InitAnchors(obj.AddComponent<RectTransform>());

			obj.AddComponent<LayoutElement>().flexibleHeight = 1;
			obj.transform.SetParent(parent.transform);

			Window window = new Window("Camera Properties")
			{
				AnchorMin = new Vector2(1, 0),
				AnchorMax = new Vector2(1, 0),
			};
			window.PositionRect.offsetMin = new Vector2(-400, 0);
			window.PositionRect.offsetMax = new Vector2(0, 600);
			window.AddTo(obj);

			GameObject propertyEditor = new GameObject();
			PropertyEditorList editorComponent = propertyEditor.AddComponent<PropertyEditorList>();
			editorComponent.AddFactory(new KeyframeEditorFactory(new BlockTimelineController(Timeline)));
			editorComponent.AddFactory(new StandardEditorFactory());
			editorComponent.SetObject(CameraProperties, Timeline);
			propertyEditor.transform.SetParent(window.Content.transform);

			Timeline.Blocks.Add(new Timeline.Blocks.Camera(CameraProperties));
			Timeline.Blocks.Add(new Timeline.Blocks.Scene());
		}

		protected void InitialiseTimeline(GameObject parent, ComponentInitialiser componentInitialiser)
		{
			GameObject obj = ComponentInitialiser.Panel("Pane");
			LayoutElement layoutElement = obj.AddComponent<LayoutElement>();
			layoutElement.flexibleHeight = 0;
			layoutElement.preferredHeight = 200;
			obj.transform.SetParent(parent.transform);

			GameObject test = new GameObject();
			test.transform.SetParent(obj.transform);
			test.AddComponent<TimelineGUI>().timeline = Timeline;
			test.GetComponent<RectTransform>().pivot = Vector2.zero;
			//test.AddComponent<LayoutElement>().minHeight = 200;
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
				DroneheimResources.Init();

				//Resources.FindObjectsOfTypeAll<Sprite>().Do(font => Debug.Log(font.name));

				Font arial = Resources.GetBuiltinResource<Font>("Arial.ttf");
				Font norse = Resources.FindObjectsOfTypeAll<Font>().First(f => f.name == "Norse");

				stylesheet.AddStyle("Body", s =>
				{
					s.Set(P.Font, norse);
					s.Set(P.FontSize, 20);
					s.Set(P.TextAlign, TextAnchor.UpperLeft);
				});

				stylesheet.AddStyle("Button", s =>
				{
					s.Set(P.BackgroundImage, DroneheimResources.ConfirmBackground);
					s.Set(P.FontStyle, FontStyle.Bold);
					s.Set(P.Color, Color.white);
					s.Set(P.Padding, new RectOffset(16, 16, 0, 0));
					s.Set(P.TextAlign, TextAnchor.MiddleCenter);
				});

				stylesheet.AddStyle(".Window", s =>
				{
					s.Set(P.BackgroundImage, DroneheimResources.WindowBackground);
					s.Set(P.Color, Color.white);
				});

				stylesheet.AddStyle(".properties", s =>
				{
					s.Set(P.Padding, new RectOffset(16, 16, 16, 16));
				});

				stylesheet.AddStyle(".Pane", s =>
				{
					s.Set(P.Padding, new RectOffset(16, 16, 16, 16));
					s.Set(P.BackgroundImage, DroneheimResources.PaneBackground);
					s.Set(P.Color, Color.white);
				});

				stylesheet.AddStyle(".Window .Title", s =>
				{
					s.Set(P.Font, norse);
					s.Set(P.FontSize, 24);
					s.Set(P.FontStyle, FontStyle.Bold);
					s.Set(P.TextAlign, TextAnchor.MiddleCenter);
					s.Set(P.Color, Color.white);
					s.Set(P.Height, 42);
					//s.Set(P.Padding, new RectOffset(16, 16, 8, 8));
				});

				stylesheet.AddStyle("Input", s =>
				{
					s.Set(P.FontStyle, FontStyle.Bold);
					s.Set(P.Padding, new RectOffset(16, 16, 16, 16));
					s.Set(P.BackgroundImage, DroneheimResources.InputBackground);
					s.Set(P.Color, RGBColor(200, 200, 200));
					s.Set(P.TextAlign, TextAnchor.MiddleCenter);
				});

				stylesheet.AddStyle(".property", s =>
				{
					s.Set(P.FontStyle, FontStyle.Normal);
					s.Set(P.Color, RGBColor(150, 150, 150));
				});

				stylesheet.AddStyle(".property .name", s =>
				{
					s.Set(P.TextAlign, TextAnchor.MiddleLeft);
				});

				stylesheet.AddStyle(".timeline-time", s =>
				{
					s.Set(P.FontSize, 12);
					s.Set(P.Font, arial);
					s.Set(P.TextAlign, TextAnchor.MiddleCenter);
				});

				stylesheet.AddStyle(".block-panel", s =>
				{
					s.Set(P.BackgroundColor, Color.clear);
				});

				stylesheet.AddStyle(".track", s =>
				{
					s.Set(P.FontStyle, FontStyle.Bold);
					s.Set(P.FontSize, 20);
					//s.Set(P.Padding, new RectOffset(16, 16, 16, 16));
					s.Set(P.BackgroundImage, DroneheimResources.InsetBackground);
					s.Set(P.Color, RGBColor(200, 200, 200));
				});

				stylesheet.AddStyle(".block", s =>
				{
					s.Set(P.BackgroundImage, DroneheimResources.KeyframeBlockBackground);
					s.Set(P.FontSize, 20);
					s.Set(P.TextAlign, TextAnchor.MiddleLeft);
					s.Set(P.Color, Color.white);
				});

				stylesheet.AddStyle(".block .label", s =>
				{
					s.Set(P.Padding, new RectOffset(8, 8, 4, 4));
					s.Set(P.TextAlign, TextAnchor.MiddleLeft);
					s.Set(P.Color, Color.white);
				});

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

				ui.transform.SetParent(transform);

				LayoutRebuilder.MarkLayoutForRebuild(transform as RectTransform);
				//GameObject.Find("_GameMain").SetActive(true);
			}
			catch (Exception)
			{
				Destroy(ui);
				throw;
			}
		}

		public void Awake()
		{
			if (GetComponent<TimelineController>() == null)
				gameObject.AddComponent<TimelineController>();

			InitialiseUI();

			if (GetComponent<SplineRenderer>() == null)
				gameObject.AddComponent<SplineRenderer>();

			GetComponent<SplineRenderer>().Spline = CameraProperties.Transform;
		}

		public void Start()
		{
			//Timeline.PlaybackController.OnFrameChanged += frame => frameText.text = $"Frame: {frame}";
		}

		protected void HandleInput()
		{

		}

		public void LateUpdate()
		{
			if (EventSystem.current.currentSelectedGameObject != null)
				Debug.Log(EventSystem.current.currentSelectedGameObject.name);
			if (toggleFollowSpline.IsDown())
			{
				Debug.Log("Entering Follow Spline Mode");
				Timeline.PlaybackController.PlayPause();
			}

			if (Timeline.PlaybackController.Playing)
			{
				SplineNode transform = CameraProperties.Transform.GetValueAt(Timeline.PlaybackController.CurrentFrame);
				GameCamera.instance.gameObject.transform.position = transform.Position + cameraShakeController.GetTranslation(CameraProperties, Timeline.PlaybackController.CurrentTime, Timeline.PlaybackController.CurrentFrame);
				GameCamera.instance.gameObject.transform.rotation = transform.Rotation * cameraShakeController.GetRotation(CameraProperties, Timeline.PlaybackController.CurrentTime, Timeline.PlaybackController.CurrentFrame);
			}

			if (addPositionKeyframe.IsDown())
			{
				Timeline.PlaybackController.Stop();
			}

			if (clearSpline.IsDown())
			{
				positionKeyframeNext = 0;
				//KeyframeController.spline.Clear();
			}
		}
	}
}
