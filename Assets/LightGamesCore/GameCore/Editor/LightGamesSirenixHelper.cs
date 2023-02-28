using System.Reflection;
using Sirenix.Utilities.Editor;
using UnityEngine;

namespace LightGamesCore.GameCore.Editor
{
    public static class LightGamesSirenixHelper
    {
        private static FieldInfo fadeGrouPush;
        private static float fadeGrouPushValue;
        private static bool isFadingGroupAnimating;
        
        public static bool IsFadingGroupAnimating
        {
            get
            {
                if (Event.current.type == EventType.Layout)
                {
                    fadeGrouPush ??= typeof(SirenixEditorGUI).GetField("fadeGrouPush", BindingFlags.Static | BindingFlags.NonPublic);
                    var currentValue = (float) fadeGrouPush.GetValue(null);
                    isFadingGroupAnimating = fadeGrouPushValue != currentValue;
                    fadeGrouPushValue = currentValue;
                }

                return isFadingGroupAnimating;
            }
        }
    }
}