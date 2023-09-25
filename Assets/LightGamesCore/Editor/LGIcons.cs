using UnityEditor;
using UnityEngine;

namespace LGCore.Editor
{
    public static class LGIcons
    {
        public static Texture2D Get(string iconName)
        {
            return AssetDatabase.LoadAssetAtPath<Texture2D>($"Assets/{LGCorePaths.Editor}/LightGamesIcons/{iconName}");
        }
    }
}
