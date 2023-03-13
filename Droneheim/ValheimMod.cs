using System;
using System.IO;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using UnityEngine;
using UnityEngine.PostProcessing;

namespace Droneheim
{
	public enum EasingMode
	{
		Linear = 0,
		EaseIn = 1,
		EaseOut = 2,
		EaseBoth = 3
	}

	public interface Interpolator<Type>
	{
		Type Interpolate(Type a, Type b, float t);
		Type Empty { get; }
	}

	interface CatmullRomType<Type>
	{
		Type GetEndHandle(Type p0, Type p1);
		Type GetStartHandle(Type p0, Type p1);
	}

	[BepInPlugin(MID, PluginName, VERSION)]
	[BepInProcess("valheim.exe")]
	[HarmonyPatch]
	public class ValheimMod : BaseUnityPlugin
	{
		private const string MID = "net.fimfiction.plugins.valheim.cameraSpline";
		private const string VERSION = "1.0.0";
		private const string PluginName = "Camera Spline";

		private static ConfigFile configFile = new ConfigFile(Path.Combine(Paths.ConfigPath, "Camera Spline.cfg"), true);

		/*private static ConfigEntry<KeyboardShortcut> toggleFollowSpline =
			configFile.Bind("Hotkeys", "Toggle_follow_spline", new KeyboardShortcut(UnityEngine.KeyCode.Y));
		private static KeyboardShortcut addPositionKeyframe = new KeyboardShortcut(UnityEngine.KeyCode.U);*/

		private static Harmony _hi;

		private static GameObject droneheim;

		private static SplineEditor splineEditor = new SplineEditor();
		private static SplineEditorController splineEditorController = new SplineEditorController(splineEditor);
		private static SplineEditorView splineEditorView = new SplineEditorView(splineEditor, splineEditorController);

		private static FlyCamera flyCamera;

		void Awake()
		{
			var harmony = new Harmony(MID);
			harmony.PatchAll();
			_hi = harmony;

			droneheim = new GameObject();
			try
			{
				droneheim.AddComponent<Droneheim>();
			}
			catch (Exception e)
			{
				GameObject.Destroy(droneheim);
				droneheim = null;
				throw e;
			}
		}

		private void OnDestroy()
		{
			_hi?.UnpatchSelf();
			GameObject.Destroy(droneheim);
			Destroy(flyCamera);
		}

		[HarmonyPatch(typeof(Player), "Update")]
		[HarmonyPostfix]
		static void Update_Postfix_Player(ref Player __instance)
		{
			splineEditor.HandleInput();
		}

		[HarmonyPatch(typeof(PostProcessingBehaviour), "OnPreCull")]
		static class PostProcessingBehaviour_OnPreCull_Patch
		{
			static void Postfix(ref VignetteComponent ___m_Vignette, ref BloomComponent ___m_Bloom, ref EyeAdaptationComponent ___m_EyeAdaptation, ref DepthOfFieldComponent ___m_DepthOfField, ref MotionBlurComponent ___m_MotionBlur, ref ColorGradingComponent ___m_ColorGrading, ref TaaComponent ___m_Taa, ref FxaaComponent ___m_Fxaa, ref AmbientOcclusionComponent ___m_AmbientOcclusion, ref ChromaticAberrationComponent ___m_ChromaticAberration, ref ScreenSpaceReflectionComponent ___m_ScreenSpaceReflection)
			{
			}
		}

		/*[HarmonyPatch(typeof(GameCamera), "Awake")]
		[HarmonyPrefix]
		static void GameCameraAwake(ref GameCamera __instance, ref bool __runOriginal)
		{
			__runOriginal = true;
			Debug.Log("Added fly camera component to GameCamera");
			flyCamera = __instance.gameObject.AddComponent<FlyCamera>();
		}*/

		[HarmonyPatch(typeof(GameCamera), "LateUpdate")]
		[HarmonyPrefix]
		static void GameCameraLateUpdate(ref GameCamera __instance, ref bool __runOriginal)
		{
			if (flyCamera == null)
			{
				Debug.Log("Added fly camera component to GameCamera");
				flyCamera = __instance.gameObject.AddComponent<FlyCamera>();
			}

			__runOriginal = true;
			if (flyCamera != null)
			{
				__runOriginal = !flyCamera.Enabled;
			}
		}

		/*[HarmonyPatch(typeof(GameCamera), "Destroy")]
		[HarmonyPrefix]
		static void GameCameraDestroy(ref GameCamera __instance, ref bool __runOriginal)
		{
			flyCamera = null;
			__runOriginal = true;
		}*/

	}

	public static class AssetUtils
	{
		/// <summary>
		///     Path separator for AssetBundles
		/// </summary>
		public const char AssetBundlePathSeparator = '$';

		/// <summary>
		///     Loads a <see cref="Texture2D"/> from file at runtime.
		/// </summary>
		/// <param name="texturePath">Texture path relative to "plugins" BepInEx folder</param>
		/// <param name="relativePath">Is the given path relative</param>
		/// <returns>Texture2D loaded, or null if invalid path</returns>
		public static Texture2D LoadTexture(string texturePath, bool relativePath = true)
		{
			string path = texturePath;

			if (relativePath)
			{
				path = Path.Combine(BepInEx.Paths.PluginPath, texturePath);
			}

			if (!File.Exists(path))
			{
				return null;
			}

			// Ensure it's a texture
			if (!path.EndsWith(".png") && !path.EndsWith(".jpg"))
			{
				throw new Exception("LoadTexture can only load png or jpg textures");
			}

			byte[] fileData = File.ReadAllBytes(path);
			Texture2D tex = new Texture2D(2, 2);
			tex.LoadImage(fileData);
			return tex;
		}

		/// <summary>
		///     Loads a <see cref="Sprite"/> from file at runtime.
		/// </summary>
		/// <param name="spritePath">Texture path relative to "plugins" BepInEx folder</param>
		/// <returns>Texture2D loaded, or null if invalid path</returns>
		public static Sprite LoadSpriteFromFile(string spritePath)
		{
			return LoadSpriteFromFile(spritePath, Vector2.zero);
		}

		/// <summary>
		///     Loads a <see cref="Sprite"/> from file at runtime.
		/// </summary>
		/// <param name="spritePath">Texture path relative to "plugins" BepInEx folder</param>
		/// <param name="pivot">The pivot to use in the resulting Sprite</param>
		/// <returns>Texture2D loaded, or null if invalid path</returns>
		public static Sprite LoadSpriteFromFile(string spritePath, Vector2 pivot)
		{
			var tex = LoadTexture(spritePath);

			if (tex != null)
			{
				return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), pivot);
			}

			return null;
		}

		/// <summary>
		///     Loads a <see cref="Sprite"/> from a file path or an asset bundle (separated by <see cref="AssetBundlePathSeparator"/>)
		/// </summary>
		/// <param name="assetPath"></param>
		/// <returns></returns>
		public static Sprite LoadSprite(string assetPath)
		{
			string path = Path.Combine(BepInEx.Paths.PluginPath, assetPath);

			if (!File.Exists(path))
			{
				return null;
			}

			// Check if asset is from a bundle or from a path
			if (path.Contains(AssetBundlePathSeparator.ToString()))
			{
				string[] parts = path.Split(AssetBundlePathSeparator);
				string bundlePath = parts[0];
				string assetName = parts[1];

				// TODO: This is very likely going to need some caching for asset bundles
				AssetBundle bundle = AssetBundle.LoadFromFile(bundlePath);
				Sprite ret = bundle.LoadAsset<Sprite>(assetName);
				bundle.Unload(false);
				return ret;
			}

			// Load texture and create sprite
			Texture2D texture = LoadTexture(path, false);

			if (!texture)
			{
				return null;
			}

			return Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), Vector2.zero);
		}
	}
}
