using UnityEngine;
using P = Droneheim.GUI.StyleProperties;

namespace Droneheim.GUI
{
	class Stylers
	{
		public StyleClass Body { get; set; }
		public StyleClass Window { get; set; }
		public StyleClass WindowTitle { get; set; }
		public StyleClass Button { get; set; }

		public Color colorInsetBackground = RGBColor(52, 50, 48);

		static Color RGBColor(int r, int g, int b)
		{
			return new Color((float)r / 255.0f, (float)g / 255.0f, (float)b / 255.0f);
		}

		public Stylers()
		{
			Texture2D tex = AssetUtils.LoadTexture("ui.png");
			int d = tex.height;
			Sprite spriteWindowBackground = Sprite.Create(tex, new Rect(d * 0, 0, d, d), new Vector2(0.5f, 0.5f), 1, 0, SpriteMeshType.FullRect, new Vector4(16, 16, 16, 16));
			Sprite spriteInsetBackground = Sprite.Create(tex, new Rect(d * 1, 0, d, d), new Vector2(0.5f, 0.5f), 1, 0, SpriteMeshType.FullRect, new Vector4(16, 16, 16, 16));
			Sprite spriteConfirmBackground = Sprite.Create(tex, new Rect(d * 2, 0, d, d), new Vector2(0.5f, 0.5f), 1, 0, SpriteMeshType.FullRect, new Vector4(16, 16, 16, 16));


			Body = new StyleClass();
			Body.Set(P.Font, Resources.GetBuiltinResource<Font>("Arial.ttf"));

			Window = new StyleClass();
			Window.Set(P.Padding, new RectOffset(16, 16, 16, 16));
			Window.Set(P.BackgroundImage, spriteWindowBackground);
			Window.Set(P.Color, Color.white);

			WindowTitle = new StyleClass();
			WindowTitle.Set(P.BackgroundImage, spriteInsetBackground);
			WindowTitle.Set(P.BackgroundColor, RGBColor(87, 82, 76));
			WindowTitle.Set(P.FontSize, 18);
			WindowTitle.Set(P.FontStyle, FontStyle.Bold);
			WindowTitle.Set(P.Color, Color.white);
			WindowTitle.Set(P.Padding, new RectOffset(16, 16, 16, 16));

			Button = new StyleClass();
			Button.Set(P.BackgroundImage, spriteConfirmBackground);
			Button.Set(P.Padding, new RectOffset(16, 16, 16, 16));
			Button.Set(P.FontStyle, FontStyle.Bold);
			Button.Set(P.Color, Color.white);
		}
	}
}
