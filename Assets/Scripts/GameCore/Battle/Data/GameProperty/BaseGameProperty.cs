using System;
using Sirenix.OdinInspector;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using LightGamesCore.GameCore.Editor;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine.Serialization;

namespace Battle.Data.GameProperty
{
    [Serializable]
    public abstract class BaseGameProperty
    {
        [InfoBox("Cannot use multiple identical properties", InfoMessageType.Error, nameof(isError))]
        [CustomValueDrawer("IconDrawer")]
        public Texture2D icon;
        
        [NonSerialized] public string scope;
        
        [HideIf("$" + nameof(needHideFixed))]
        [CustomValueDrawer("FixedDrawer")]
        public float value;
        
        [HideIf("$" + nameof(NeedHidePercent))]
        [PropertyRange(0, 100)]
        public int percent;

        [HideInInspector] public bool isError;
        private bool IsRadius => GetType() == typeof(RadiusGP);
        private bool IsMoveSpeed => GetType() == typeof(MoveSpeedGP);
        private bool NeedHidePercent => IsMoveSpeed || IsRadius;
        
        
#if UNITY_EDITOR
        [HideInInspector] public string moveSpeed;
        [HideInInspector] public bool needHideFixed;
        [HideInInspector] public int index;
        private static Texture2D oddTexture;
        private static Texture2D evenTexture;
        
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

            oddTexture ??= EditorUtils.GetTextureByColor(new Color(0.24f, 0.24f, 0.24f));
            evenTexture ??= EditorUtils.GetTextureByColor(new Color(0.2f, 0.2f, 0.2f));
            moveSpeed ??= "Slow";
        }

        private float FixedDrawer(float val, GUIContent label, Func<GUIContent, bool> callNextDrawer)
        {
            if (IsRadius)
            {
                var newValue = EditorGUILayout.Slider(label, val, 1, 20);
                var roundValue = Mathf.Round(newValue * 2);
                roundValue /= 2;
                value = roundValue;
            }
            else if(IsMoveSpeed)
            {
                EditorGUILayout.BeginHorizontal();
                
                EditorGUILayout.LabelField("Move Speed", GUILayoutOptions.MaxWidth(100));
                MoveSpeedSelector.SetValue(moveSpeed);
                
                if (EditorGUILayout.DropdownButton(new GUIContent(MoveSpeedSelector.Value), FocusType.Passive))
                {
                    var newValue = new MoveSpeedSelector();
                    newValue.ShowInPopup();
                    newValue.SelectionConfirmed += x =>
                    {
                        newValue.GetValue();
                        moveSpeed = MoveSpeedSelector.Value;
                        value = MoveSpeedSelector.Speed;
                    };
                }
                
                EditorGUILayout.EndHorizontal();
            }
            else
            {
                value = EditorGUILayout.FloatField(label, val);
            }
            
            return value;
        }

        private Texture2D IconDrawer(Texture2D value, GUIContent label, Func<GUIContent, bool> callNextDrawer)
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

    public class MoveSpeedSelector : OdinSelector<string>
    {
        public static float Speed => Values[Value];
        public static string Value { get; private set; } = "Slow";
        public static Dictionary<string, float> Values = new()
        {
            {"Slow", 0.75f},
            {"Normal", 1f},
            {"Fast", 1.5f},
            {"Faster", 1.75f},
        };

        protected override void BuildSelectionTree(OdinMenuTree tree)
        {
            tree.Config.DrawSearchToolbar = false;
            foreach (var value in Values.Keys)
            {
                tree.Add(value, value);
            }
        }
        
        public static void SetValue(string value)
        {
            if (Values.ContainsKey(value))
            {
                Value = value;
            }
        }
        
        public void GetValue()
        {
            Value = GetCurrentSelection().FirstOrDefault();
        }
    }
}
#endif