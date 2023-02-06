using UnityEngine;

namespace Attributes.Editor.Drawers
{
    public partial class ColoredFieldPropertyDrawer
    {
        private void DrawBack()
        {
            var valueRect = Property.LastDrawnValueRect;
            var width = (int) valueRect.width;
            var height = (int) valueRect.height;
            var wasResize = false;

            if (width > lastWidth)
            {
                wasResize = true;
                widthChankCount++;
                ResizeAndCopyTexture();

                for (int i = 0; i < heightChankCount; i++)
                {
                    Graphics.CopyTexture(realTexture, 0, 0, 0, 0, realTexture.width, realTexture.height,
                        texture, 0, 0,
                        realTexture.width * (widthChankCount - 1),
                        realTexture.height * i);
                }

                Graphics.CopyTexture(texture, 0, 0, 0, 0, texture.width, texture.height, lastTexture, 0, 0, 0, 0);
            }

            if (height > lastHeight)
            {
                wasResize = true;
                heightChankCount++;
                ResizeAndCopyTexture();

                for (int i = 0; i < widthChankCount; i++)
                {
                    Graphics.CopyTexture(realTexture, 0, 0, 0, 0, realTexture.width, realTexture.height,
                        texture, 0, 0,
                        realTexture.width * i,
                        realTexture.height * (heightChankCount - 1));
                }

                Graphics.CopyTexture(texture, 0, 0, 0, 0, texture.width, texture.height, lastTexture, 0, 0, 0, 0);
            }

            lastTexture.Reinitialize(width, height, TextureFormat.RGBA32, false);
            lastTexture.Apply();

            if (!wasResize)
            {
                Graphics.CopyTexture(texture, 0, 0, 0, 0, width, height, lastTexture, 0, 0, 0, 0);
            }
        }
    }
}