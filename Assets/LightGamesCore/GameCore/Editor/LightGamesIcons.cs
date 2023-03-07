#if UNITY_EDITOR
using UnityEngine;

namespace LightGamesCore.GameCore.Editor
{
    public static class LightGamesIcons
    {
        public static Texture2D Get(string iconName)
        {
            return Resources.Load<Texture2D>($"LightGamesIcons/{iconName}");
        }
    }
}
#endif