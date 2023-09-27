using UnityEditor;
using UnityEngine;

namespace LGCore.Editor
{
    public static class LGIcons
    {
        public static readonly string[] imageExtensions = { ".png", ".jpg"};
        public static Texture2D Get(string iconName)
        {
            Texture2D tex = null;
            
            for (int i = 0; i < imageExtensions.Length && tex == null; i++)
            {
                tex = AssetDatabase.LoadAssetAtPath<Texture2D>($"Assets/{LGCorePaths.Editor}/LightGamesIcons/{iconName}{imageExtensions[i]}");
            }

            return tex;
        }
    }
}
