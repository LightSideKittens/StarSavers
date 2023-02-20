using UnityEngine;

namespace Core.Extensions
{
  public static class ColorExtensions
  {
    /// <summary>
    /// Returns a new color equal to the given one with changed brightness
    /// </summary>
    /// <param name="color">Color to evaluate</param>
    /// <param name="brightnessFactor">Brightness factor (multiplied by current brightness)</param>
    /// <param name="alpha">If set applies this alpha value</param>
    public static Color CloneAndChangeBrightness(
      this Color color,
      float brightnessFactor,
      float? alpha = null)
    {
      float H;
      float S;
      float V1;
      ColorExtensions.RGBToHSV(color, out H, out S, out V1);
      float V2 = V1 * brightnessFactor;
      if ((double) V2 < 0.0)
        V2 = 0.0f;
      else if ((double) V2 > 1.0)
        V2 = 1f;
      Color rgb = ColorExtensions.HSVToRGB(H, S, V2);
      if (alpha.HasValue)
        rgb.a = alpha.Value;
      return rgb;
    }

    /// <summary>
    /// Returns a new color equal to the given one with changed saturation
    /// </summary>
    /// <param name="color">Color to evaluate</param>
    /// <param name="saturationFactor">Saturation factor (multiplied by current brightness)</param>
    /// <param name="alpha">If set applies this alpha value</param>
    public static Color CloneAndChangeSaturation(
      this Color color,
      float saturationFactor,
      float? alpha = null)
    {
      float H;
      float S1;
      float V;
      ColorExtensions.RGBToHSV(color, out H, out S1, out V);
      float S2 = S1 * saturationFactor;
      if ((double) S2 < 0.0)
        S2 = 0.0f;
      else if ((double) S2 > 1.0)
        S2 = 1f;
      Color rgb = ColorExtensions.HSVToRGB(H, S2, V);
      if (alpha.HasValue)
        rgb.a = alpha.Value;
      return rgb;
    }

    /// <summary>Changes the alpha of this color and returns it</summary>
    public static Color SetAlpha(this Color color, float alpha)
    {
      color.a = alpha;
      return color;
    }

    /// <summary>
    /// Returns a new color equal to the given one with changed alpha
    /// </summary>
    public static Color CloneAndSetAlpha(this Color color, float alpha) => new Color(color.r, color.g, color.b, alpha);

    /// <summary>
    /// Returns a HEX version of the given Unity Color, without the initial #
    /// </summary>
    /// <param name="includeAlpha">If TRUE, also converts the alpha value and returns a hex of 8 characters,
    /// otherwise doesn't and returns a hex of 6 characters</param>
    public static string ToHex(this Color32 color, bool includeAlpha = false)
    {
      string hex = color.r.ToString("x2") + color.g.ToString("x2") + color.b.ToString("x2");
      if (includeAlpha)
        hex += color.a.ToString("x2");
      return hex;
    }

    /// <summary>
    /// Returns a HEX version of the given Unity Color, without the initial #
    /// </summary>
    /// <param name="includeAlpha">If TRUE, also converts the alpha value and returns a hex of 8 characters,
    /// otherwise doesn't and returns a hex of 6 characters</param>
    public static string ToHex(this Color color, bool includeAlpha = false) => ((Color32) color).ToHex(includeAlpha);

    private static void RGBToHSV(Color rgbColor, out float H, out float S, out float V)
    {
      if ((double) rgbColor.b > (double) rgbColor.g && (double) rgbColor.b > (double) rgbColor.r)
        ColorExtensions.RGBToHSVHelper(4f, rgbColor.b, rgbColor.r, rgbColor.g, out H, out S, out V);
      else if ((double) rgbColor.g > (double) rgbColor.r)
        ColorExtensions.RGBToHSVHelper(2f, rgbColor.g, rgbColor.b, rgbColor.r, out H, out S, out V);
      else
        ColorExtensions.RGBToHSVHelper(0.0f, rgbColor.r, rgbColor.g, rgbColor.b, out H, out S, out V);
    }

    private static void RGBToHSVHelper(
      float offset,
      float dominantcolor,
      float colorone,
      float colortwo,
      out float H,
      out float S,
      out float V)
    {
      V = dominantcolor;
      if ((double) V != 0.0)
      {
        float num1 = (double) colorone <= (double) colortwo ? colorone : colortwo;
        float num2 = V - num1;
        if ((double) num2 != 0.0)
        {
          S = num2 / V;
          H = offset + (colorone - colortwo) / num2;
        }
        else
        {
          S = 0.0f;
          H = offset + (colorone - colortwo);
        }
        H /= 6f;
        if ((double) H >= 0.0)
          return;
        ++H;
      }
      else
      {
        S = 0.0f;
        H = 0.0f;
      }
    }

    private static Color HSVToRGB(float H, float S, float V, bool hdr = true)
    {
      Color white = Color.white;
      if ((double) S == 0.0)
      {
        white.r = V;
        white.g = V;
        white.b = V;
      }
      else if ((double) V == 0.0)
      {
        white.r = 0.0f;
        white.g = 0.0f;
        white.b = 0.0f;
      }
      else
      {
        white.r = 0.0f;
        white.g = 0.0f;
        white.b = 0.0f;
        double f = (double) H * 6.0;
        int num1 = (int) Mathf.Floor((float) f);
        float num2 = (float) f - (float) num1;
        float num3 = V * (1f - S);
        float num4 = V * (float) (1.0 - (double) S * (double) num2);
        float num5 = V * (float) (1.0 - (double) S * (1.0 - (double) num2));
        switch (num1)
        {
          case -1:
            white.r = V;
            white.g = num3;
            white.b = num4;
            break;
          case 0:
            white.r = V;
            white.g = num5;
            white.b = num3;
            break;
          case 1:
            white.r = num4;
            white.g = V;
            white.b = num3;
            break;
          case 2:
            white.r = num3;
            white.g = V;
            white.b = num5;
            break;
          case 3:
            white.r = num3;
            white.g = num4;
            white.b = V;
            break;
          case 4:
            white.r = num5;
            white.g = num3;
            white.b = V;
            break;
          case 5:
            white.r = V;
            white.g = num3;
            white.b = num4;
            break;
          case 6:
            white.r = V;
            white.g = num5;
            white.b = num3;
            break;
        }
        if (!hdr)
        {
          white.r = Mathf.Clamp(white.r, 0.0f, 1f);
          white.g = Mathf.Clamp(white.g, 0.0f, 1f);
          white.b = Mathf.Clamp(white.b, 0.0f, 1f);
        }
      }
      return white;
    }
  }
}
