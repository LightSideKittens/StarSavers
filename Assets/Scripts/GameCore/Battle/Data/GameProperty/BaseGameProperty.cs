using System;
using System.Collections.Generic;
using System.Reflection;
using LightGamesCore.GameCore.Editor;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEngine;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;

namespace Battle.Data.GameProperty
{
    [Serializable]
    public abstract class BaseGameProperty
    {
        [FormerlySerializedAs("heartIcon")]
        [InfoBox("Cannot use multiple identical properties", InfoMessageType.Error, nameof(isError))]
        [CustomValueDrawer(nameof(HeartIconDrawer))]
        public Texture2D icon;
        
        [HideInInspector] public string scope;
        [HideIf("$" + nameof(needHideFixed))]
        public float Fixed;
        [Range(0, 1)] public float Percent;

        [HideInInspector] public bool needHideFixed;
        [HideInInspector] public bool isError;
        private Texture2D oddTexture;
        private Texture2D evenTexture;
        
        private int index;
        private static int currentIndex;
        private static HashSet<Type> types = new();
        private static Dictionary<Type, string> iconsByType = new()
        {
            {typeof(HealthGP), "health-icon"},
            {typeof(DamageGP), "attack-icon"},
            {typeof(AttackSpeedGP), "attack-speed-icon"},
            {typeof(MoveSpeedGP), "speed-icon"},
            {typeof(RadiusGP), "radius-icon"},
        };
        
        private string Title => GetType().Name.Replace("GP", "Property").SplitPascalCase();

        [OnInspectorDispose]
        private void OnInspectorDispose()
        {
            types.Clear();
            currentIndex = 0;
            index = 0;
        }
        
        [OnInspectorInit]
        private void CreateData()
        {
            if (isError)
            {
                return;
            }
            
            if (!types.Add(GetType()))
            {
                currentIndex = 0;
                types.Clear();
            }
            
            currentIndex++;
            index = currentIndex;

            if (iconsByType.TryGetValue(GetType(), out var tex))
            {
                icon = LightGamesIcons.Get(tex);
            }

            oddTexture = EditorUtils.GetTextureByColor(new Color(0.24f, 0.24f, 0.24f));
            evenTexture = EditorUtils.GetTextureByColor(new Color(0.2f, 0.2f, 0.2f));
        }

        private Texture2D HeartIconDrawer(Texture2D value, GUIContent label, Func<GUIContent, bool> callNextDrawer)
        {
            if (isError || icon == null)
            {
                return icon;
            }

            var rect = GUIHelper.GetCurrentLayoutRect();

            rect.xMin += 40;
            rect.yMax = rect.yMin + 30;
            var texRect = rect;
            
            GUI.DrawTexture(texRect, index % 2 == 0 ? evenTexture : oddTexture, ScaleMode.StretchToFill, false);
            
            var textStyle = new GUIStyle() {alignment = TextAnchor.MiddleCenter};
            textStyle.normal.textColor = Color.white;
            textStyle.richText = true;
            GUI.Label(rect, $"<b>{Title}</b>", textStyle);
            
            var lastPosition = rect.position;
            rect.position = lastPosition + new Vector2(rect.height - 10, 0);
            GUI.Box(rect, icon, GUIStyle.none);
            rect.position = lastPosition + new Vector2(rect.width - rect.height - 20, 0);
            GUI.Box(rect, icon, GUIStyle.none);
            GUILayout.Space(10);

            return icon;
        }
    }
}