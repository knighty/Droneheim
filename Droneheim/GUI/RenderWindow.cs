using UnityEngine;
using UnityEngine.UI;

namespace Droneheim.GUI
{
	[RequireComponent(typeof(RectTransform))]
	public class RenderWindow : MonoBehaviour, ILayoutElement
	{
		public float minWidth => -1;

		public float preferredWidth => -1;

		public float flexibleWidth => 1;

		public float minHeight => -1;

		public float preferredHeight => -1;

		public float flexibleHeight => 1;

		public int layoutPriority => 1;

		RenderTexture renderTexture;
		Camera camera;

		public void CalculateLayoutInputHorizontal()
		{
		}

		public void CalculateLayoutInputVertical()
		{
		}

		protected void Destroy()
		{
			renderTexture?.Release();
			GameObject.Destroy(camera?.gameObject);
		}

		protected Camera InitCamera(RenderTexture renderTexture)
		{
			GameObject cameraObject = new GameObject();

			Camera camera = cameraObject.AddComponent<Camera>();
			camera.CopyFrom(Camera.main);
			camera.targetTexture = renderTexture;
			//camera.cullingMask &= ~(LayerMask.NameToLayer("UI"));
			camera.transform.position = Camera.main.transform.position;

			cameraObject.AddComponent<FlyCamera>();

			return camera;
		}

		protected void InitRenderer()
		{
			GameObject render = new GameObject();
			render.transform.SetParent(transform);
			RawImage rawImage = render.AddComponent<RawImage>();
			rawImage.texture = camera.targetTexture;
			ComponentInitialiser.InitAnchors(render.GetComponent<RectTransform>(), Vector2.zero, Vector2.one);
			render.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
		}

		protected RenderTexture InitRenderTarget()
		{
			RenderTexture renderTexture = new RenderTexture(1920, 1080, 16, RenderTextureFormat.ARGB32);
			renderTexture.Create();
			return renderTexture;
		}

		protected void Awake()
		{
			//renderTexture = InitRenderTarget();
			//camera = InitCamera(renderTexture);
			//InitRenderer();

			/*GameObject render = new GameObject();
			render.transform.SetParent(transform);
			RawImage rawImage = render.AddComponent<RawImage>();
			rawImage.texture = DroneheimResources.SPR;
			ComponentInitialiser.InitAnchors(render.GetComponent<RectTransform>(), Vector2.zero, Vector2.one);
			render.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;*/

			/*AssetBundle bundle = Jotunn.Utils.AssetUtils.LoadAssetBundleFromResources("Shaders");
			bundle.LoadAsset<Material>("AccumulateFrames");*/
		}
	}
}
