using UnityEngine;
using UnityEngine.UI;

namespace ShadowBox.Extensions
{
	public static class ShadowedColor
	{
		public static Color RandomBright
		{
			get { return new Color(Random.Range(.4f, 1), Random.Range(.4f, 1), Random.Range(.4f, 1)); }
		}

		public static Color RandomDim
		{
			get { return new Color(Random.Range(.4f, .6f), Random.Range(.4f, .8f), Random.Range(.4f, .8f)); }
		}

		public static Color RandomColor
		{
			get { return new Color(Random.Range(.1f, .9f), Random.Range(.1f, .9f), Random.Range(.1f, .9f)); }
		}
		/// <summary>
		/// Returns new Color with Alpha set to a
		/// </summary>
		public static Color WithAlphaSetTo(this Color color, float a)
		{
			return new Color(color.r, color.g, color.b, a);
		}

		/// <summary>
		/// Set Alpha of Image.Color
		/// </summary>
		public static void SetAlpha(this Image image, float a)
		{
			var color = image.color;
			color = new Color(color.r, color.g, color.b, a);
			image.color = color;
		}

		/// <summary>
		/// Set Alpha of Renderer.Color
		/// </summary>
		public static void SetAlpha(this SpriteRenderer renderer, float a)
		{
			var color = renderer.color;
			color = new Color(color.r, color.g, color.b, a);
			renderer.color = color;
		}

		/// <summary>
		/// To string of "#b5ff4f" format
		/// </summary>
		public static string ToHex(this Color color)
		{
			return string.Format("#{0:X2}{1:X2}{2:X2}", (int) (color.r * 255), (int) (color.g * 255), (int) (color.b * 255));
		}

		/// <summary>
		/// Returns a new UnityEngine.Color for string hex of "#b5ff4f" format.
		/// </summary>
		public static Color ToColor(this string hex)
		{
			Color x;
			ColorUtility.TryParseHtmlString(hex, out x);
			return x;
		}

		public static Color32 ToColor32(this string hex)
        {
			hex = hex.Replace("0x", "");//in case the string is formatted 0xFFFFFF
			hex = hex.Replace("#", "");//in case the string is formatted #FFFFFF
			byte a = 255;//assume fully visible unless specified in hex
			byte r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
			byte g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
			byte b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
			//Only use alpha if the string has enough characters
			if (hex.Length == 8)
			{
				a = byte.Parse(hex.Substring(6, 2), System.Globalization.NumberStyles.HexNumber);
			}

			return new Color32(r, g, b, a);
		}

		public static string ShadowedPurple = "#6523A3";

		private const float LightOffset = 0.0625f;
		private const float DarkerFactor = 0.9f;
		/// <summary>
		/// Returns a color lighter than the given color.
		/// </summary>
		/// <param name="color"></param>
		/// <returns></returns>
		public static Color Lighter(this Color color)
		{
			return new Color(
				color.r + LightOffset,
				color.g + LightOffset,
				color.b + LightOffset,
				color.a);
		}

		/// <summary>
		/// Returns a color darker than the given color.
		/// </summary>
		/// <param name="color"></param>
		/// <returns></returns>
		public static Color Darker(this Color color)
		{
			return new Color(
				color.r - LightOffset,
				color.g - LightOffset,
				color.b - LightOffset,
				color.a);
		}

		
		/// <summary>
		/// Brightness offset with 1 is brightest and -1 is darkest
		/// </summary>
		public static Color BrightnessOffset(this Color color, float offset)
		{
			return new Color(
				color.r + offset,
				color.g + offset,
				color.b + offset,
				color.a);
		}
	}
}