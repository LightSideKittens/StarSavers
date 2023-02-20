using Core.Extensions;
using UnityEngine;

public static class EditorUtils
{
    public struct ColorData
    {
        public Color color;
        public bool isDark;

        public ColorData(Color color, float brightFactor = 0.8f, float saturationFactor = 0.5f)
        {
            this.color = color.CloneAndChangeSaturation(saturationFactor);
            this.color = this.color.CloneAndChangeBrightness(brightFactor);
            this.isDark = this.color.grayscale <= 0.5f;
        }
    }

    public static ColorData[] Colors { get; } = new[]
    {
        new ColorData(Color.red),
        new ColorData(Color.yellow),
        new ColorData(Color.blue),
        new ColorData(Color.green),
        new ColorData(Color.cyan),
        new ColorData(Color.magenta, 1, 0.3f),
        new ColorData(Color.gray),
        new ColorData(Color.white),
    };

    public static ColorData GetColorData(int index)
    {
        return Colors[index % Colors.Length];
    }

    public static Texture2D GetTextureByColor(Color color)
    {
        var texture = new Texture2D(16, 16);
        var pixels = 16 * 16;
        var colors = new Color[pixels];

        for (int i = 0; i < pixels; i++)
        {
            colors[i] = color;
        }

        texture.SetPixels(colors);
        texture.Apply();
        
        return texture;
    }
}
