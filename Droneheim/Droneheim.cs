using BepInEx.Configuration;
using Droneheim.Commands;
using Droneheim.GUI;
using Droneheim.GUI.Properties;
using Droneheim.Spline;
using Droneheim.Timeline;
using System;
using System.Linq;
using System.Net;
using UnityEngine;
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
		public static Sprite KeyframeBlockBackgroundSelected;
		public static Sprite HorizontalPaneBackground;

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

		public static Texture2D SPR;

		public static void Init()
		{
			//Texture2D sprites = AssetUtils.LoadTexture("ui.png");
			//Texture2D panels = AssetUtils.LoadTexture("ui2.png");

			AssetBundle bundle = Jotunn.Utils.AssetUtils.LoadAssetBundleFromResources("Assets.ui");
			ValheimMod.assetBundle = bundle;
			Texture2D sprites = bundle.LoadAsset<Texture2D>("assets/ui.png");
			Texture2D panels = bundle.LoadAsset<Texture2D>("assets/ui2.png");

			SPR = sprites;

			//Debug.Log(sprites.GetPixel(20, 20));
			//Debug.Log(sprites.);

			Sprite GetSprite(Texture2D tex, int x, int y, int size)
			{
				return Sprite.Create(tex, new Rect(x, y, size, size), new Vector2(0.5f, 0.5f), 1, 0);
			}

			Sprite GetSlicedSprite(Texture2D tex, int x, int y, int size, Vector4 slicedBorder)
			{
				return Sprite.Create(tex, new Rect(x, y, size, size), new Vector2(0.5f, 0.5f), 1, 0, SpriteMeshType.FullRect, slicedBorder);
			}

			int full = 64;
			int half = 32;

			Vector4 border16 = new Vector4(16, 16, 16, 16);

			PaneBackground = GetSlicedSprite(sprites, full * 0, 0, full, border16);
			HorizontalPaneBackground = Sprite.Create(sprites, new Rect(full * 0 + 16, 0, 32, 64), new Vector2(0.5f, 0.5f), 1, 0, SpriteMeshType.FullRect, new Vector4(8, 16, 8, 16));
			WindowBackground = GetSlicedSprite(panels, 0, 128 * 3, 128, new Vector4(24, 8, 24, 64));
			InsetBackground = GetSlicedSprite(sprites, full * 1, 0, full, border16);
			ConfirmBackground = GetSlicedSprite(sprites, full * 2, 0, full, border16);
			InputBackground = GetSlicedSprite(sprites, full * 4, 0, full, border16);
			KeyframeBlockBackground = GetSlicedSprite(sprites, full * 8, 0, full, new Vector4(4, 32, 4, 4));
			KeyframeBlockBackgroundSelected = GetSlicedSprite(sprites, full * 9, 0, full, new Vector4(4, 32, 4, 4));

			KeyframeOff = GetSprite(sprites, full * 3, half, half);
			KeyframeOn = GetSprite(sprites, full * 3, 0, half);

			PreviousKeyframe = GetSprite(sprites, full * 3 + half * 1, 0, half);
			NextKeyframe = GetSprite(sprites, full * 3 + half * 1, half, half);

			Collapse = GetSprite(sprites, full * 5 + half, 0, half);
			Expand = GetSprite(sprites, full * 5 + half, half, half);

			Play = GetSprite(sprites, full * 6, half, half);
			Pause = GetSprite(sprites, full * 6, 0, half);

			Next = GetSprite(sprites, full * 6 + half, half, half);
			Previous = GetSprite(sprites, full * 6 + half, 0, half);

			Stop = GetSprite(sprites, full * 7, half, half);
			//Previous = GetSprite(tex, full * 7, 0, half);

			Eye = GetSprite(sprites, full * 7 + half, half, half);
			EyeSlash = GetSprite(sprites, full * 7 + half, 0, half);

			Checkmark = EyeSlash;// Resources.FindObjectsOfTypeAll<Sprite>().First(f => f.name == "Checkmark");

			TimelineScrubber = GetSprite(sprites, full * 5, half, half);
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
			GameObject obj = ComponentInitialiser.Panel("menu-panel");
			obj.AddComponent<LayoutElement>().flexibleHeight = 0;
			obj.transform.SetParent(parent.transform);
		}

		protected void InitialiseCentre(GameObject parent, ComponentInitialiser componentInitialiser)
		{
			GameObject layout = new GameObject();
			ComponentInitialiser.InitAnchors(layout.AddComponent<RectTransform>());
			layout.AddComponent<LayoutElement>().flexibleHeight = 1;
			layout.transform.SetParent(parent.transform);
			HorizontalLayoutGroup layoutGroup = layout.AddComponent<HorizontalLayoutGroup>();
			layoutGroup.childForceExpandWidth = false;
			layoutGroup.childControlWidth = true;

			GameObject renderWindowObject = new GameObject();
			renderWindowObject.transform.SetParent(layout.transform);
			renderWindowObject.AddComponent<RenderWindow>();

			Window window = new Window("Camera Properties")
			{
			//	AnchorMin = new Vector2(1, 0),
			//	AnchorMax = new Vector2(1, 0),
			};
			//window.PositionRect.offsetMin = new Vector2(-400, 0);
			//window.PositionRect.offsetMax = new Vector2(0, 600);
			window.AddTo(layout);

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
			GameObject obj = ComponentInitialiser.Panel("timeline-panel");
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
			ui.layer = LayerMask.NameToLayer("UI");
			try
			{
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

				stylesheet.AddStyle(".Window", s =>
				{
					s.Set(P.BackgroundImage, DroneheimResources.WindowBackground);
					s.Set(P.Color, Color.white);
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

				// Controls
				stylesheet.AddStyle("Input", s =>
				{
					s.Set(P.FontStyle, FontStyle.Bold);
					s.Set(P.Padding, new RectOffset(16, 16, 16, 16));
					s.Set(P.BackgroundImage, DroneheimResources.InputBackground);
					s.Set(P.Color, RGBColor(200, 200, 200));
					s.Set(P.TextAlign, TextAnchor.MiddleCenter);
				});

				stylesheet.AddStyle("Button", s =>
				{
					s.Set(P.BackgroundImage, DroneheimResources.ConfirmBackground);
					s.Set(P.FontStyle, FontStyle.Bold);
					s.Set(P.Color, Color.white);
					s.Set(P.Padding, new RectOffset(16, 16, 0, 0));
					s.Set(P.TextAlign, TextAnchor.MiddleCenter);
				});

				// Menu
				stylesheet.AddStyle(".menu-panel", s =>
				{
					s.Set(P.BackgroundImage, DroneheimResources.HorizontalPaneBackground);
				});

				// Properties
				stylesheet.AddStyle(".properties", s =>
				{
					s.Set(P.Padding, new RectOffset(16, 16, 16, 16));
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

				// Timeline
				stylesheet.AddStyle(".timeline-panel", s =>
				{
					s.Set(P.Color, Color.white);
					s.Set(P.Padding, new RectOffset(16, 16, 16, 16));
					s.Set(P.BackgroundImage, DroneheimResources.HorizontalPaneBackground);
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

				stylesheet.AddStyle(".block:focus", s =>
				{
					s.Set(P.BackgroundImage, DroneheimResources.KeyframeBlockBackgroundSelected);
				});

				stylesheet.AddStyle(".block .label", s =>
				{
					s.Set(P.Padding, new RectOffset(8, 8, 4, 4));
					s.Set(P.TextAlign, TextAnchor.MiddleLeft);
					s.Set(P.Color, Color.white);
				});

				stylesheet.AddStyle(".timeline-track-label", s =>
				{
					s.Set(P.BackgroundImage, DroneheimResources.InputBackground);
					s.Set(P.FontSize, 24);
					s.Set(P.TextAlign, TextAnchor.MiddleCenter);
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
				//mainLayoutGroup.padding = new RectOffset(16, 16, 16, 16);
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

			CommandList.Instance.OnAdd += command => Debug.Log($"Added command: {command}");
			CommandList.Instance.OnUndo += command => Debug.Log($"Undo command: {command}");
			CommandList.Instance.OnRedo += command => Debug.Log($"Redo command: {command}");
		}

		public void Start()
		{
			//Timeline.PlaybackController.OnFrameChanged += frame => frameText.text = $"Frame: {frame}";
		}

		protected void HandleInput()
		{

		}

		protected void Update()
		{
			if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.Z))
			{
				CommandList.Instance.Undo();
			}
			if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.Y))
			{
				CommandList.Instance.Redo();
			}
		}

		public void LateUpdate()
		{
			/*if (EventSystem.current.currentSelectedGameObject != null)
				Debug.Log(EventSystem.current.currentSelectedGameObject.name);*/
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
