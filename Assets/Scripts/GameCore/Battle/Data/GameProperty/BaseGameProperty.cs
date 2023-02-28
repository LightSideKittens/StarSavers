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

namespace Battle.Data.GameProperty
{
    [Serializable]
    public abstract class BaseGameProperty
    {
        [CustomValueDrawer("IconDrawer")]
        public Texture2D icon;
        
        [NonSerialized] public string scope;
        
        [HideIf("$" + nameof(needHideFixed))]
        [CustomValueDrawer("FixedDrawer")]
        public float value;
        
        [HideIf("$" + nameof(NeedHidePercent))]
        [PropertyRange(0, 100)]
        public int percent;
        
        private bool IsRadius => GetType() == typeof(RadiusGP);
        private bool IsMoveSpeed => GetType() == typeof(MoveSpeedGP);
        private bool IsAttackSpeed => GetType() == typeof(AttackSpeedGP);
        private bool NeedHidePercent => IsMoveSpeed || IsRadius || IsAttackSpeed;
        
        
#if UNITY_EDITOR
        public static bool isInited;
        private static Texture2D evenTexture;
        public static Dictionary<Type, string> IconsByType { get; } = new()
        {
            {typeof(HealthGP), "health-icon"},
            {typeof(DamageGP), "attack-icon"},
            {typeof(AttackSpeedGP), "attack-speed-icon"},
            {typeof(MoveSpeedGP), "speed-icon"},
            {typeof(RadiusGP), "radius-icon"},
        };
        
        [HideInInspector] public string moveSpeed = "Slow";
        [HideInInspector] public bool needHideFixed;

        private string Title => GetType().Name.Replace("GP", "Property").SplitPascalCase();
        
        protected BaseGameProperty()
        {
            if (isInited)
            {
                CreateData();
            }
        }

        [OnInspectorDispose]
        private void OnInspectorDispose()
        {
            isInited = false;
        }
        
        [OnInspectorInit]
        private void CreateData()
        {
            isInited = true;

            if (IconsByType.TryGetValue(GetType(), out var tex))
            {
                icon = LightGamesIcons.Get(tex);
            }

            if (evenTexture == null)
            {
                evenTexture = EditorUtils.GetTextureByColor(new Color(0.17f, 0.17f, 0.18f));
            }

            if (IsMoveSpeed)
            {
                MoveSpeedSelector.SetValue(moveSpeed);
                value = MoveSpeedSelector.Speed;
            }
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
            else if(IsMoveSpeed && moveSpeed != null)
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
            else if(IsAttackSpeed)
            {
                if (value == 0)
                {
                    value = 1;
                }
                
                var binary = Convert.ToString((int)value, 2);
                var newBinary = string.Empty;
                
                EditorGUILayout.BeginHorizontal();
                foreach (var bit in binary)
                {
                    newBinary += EditorGUILayout.Toggle(bit == '1', GUILayoutOptions.ExpandWidth(false)) ? '1' : '0';
                }
                EditorGUILayout.EndHorizontal();

                if (GUILayout.Button("Add"))
                {
                    if (newBinary.Length < 8)
                    {
                        newBinary += '0';
                    }
                }
                
                value = Convert.ToInt32(newBinary, 2);
            }
            else
            {
                value = EditorGUILayout.FloatField(label, val);
            }

            return value;
        }

        private Texture2D IconDrawer(Texture2D value, GUIContent label, Func<GUIContent, bool> callNextDrawer)
        {
            if (icon == null)
            {
                return icon;
            }

            var rect = GUIHelper.GetCurrentLayoutRect();

            rect.xMax -= 20;
            rect.xMin += 40;
            rect.yMax = rect.yMin + 30;
            var texRect = rect;
            var center = texRect.center;
            texRect.height -= 8;
            texRect.width -= 8;
            texRect.center = center;
            
            GUI.DrawTexture(texRect, evenTexture, ScaleMode.StretchToFill, false);
            
            var textStyle = new GUIStyle() {alignment = TextAnchor.MiddleCenter};
            textStyle.normal.textColor = Color.white;
            textStyle.richText = true;
            GUI.Label(rect, $"<b>{Title}</b>", textStyle);
            
            var lastPosition = rect.position;
            rect.position = lastPosition;
            GUI.Box(rect, icon, GUIStyle.none);
            rect.position = lastPosition + new Vector2(rect.width - rect.height, 0);
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