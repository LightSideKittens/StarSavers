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
        public decimal value;
        
        [HideIf("$" + nameof(NeedHidePercent))]
        [PropertyRange(0, 100)]
        public int percent;

        public static decimal operator +(decimal a, BaseGameProperty b)
        {
            return b.ComputeValue(a);
        }
        
        public virtual decimal ComputeValue(decimal val)
        {
            return value + val;
        }
        
        private bool IsRicochet => GetType() == typeof(RicochetGP);
        private bool IsRadius => GetType() == typeof(RadiusGP) || GetType() == typeof(AreaDamageGP);
        private bool IsMoveSpeed => GetType() == typeof(MoveSpeedGP);
        private bool IsAttackSpeed => GetType() == typeof(AttackSpeedGP);
        private bool IsHealth => GetType() == typeof(HealthGP);
        private bool IsEffector => string.IsNullOrEmpty(scope) || scope.Contains("Effectors");
        private bool NeedHidePercent => (IsMoveSpeed && !IsEffector) || IsRadius || IsAttackSpeed || IsRicochet;
        
        
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
            {typeof(AreaDamageGP), "area-damage-icon"},
            {typeof(RicochetGP), "ricochet-icon"},
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

            if (IsMoveSpeed && !IsEffector)
            {
                MoveSpeedSelector.SetValue(moveSpeed);
                value = (decimal)MoveSpeedSelector.Speed;
            }
        }

        private decimal DrawRadiusSlider(decimal val, GUIContent label)
        {
            var newValue = EditorGUILayout.Slider(label, (float)val, 0.5f, 16);
            var roundValue = Mathf.Round(newValue * 2);
            roundValue /= 2;

            return (decimal)roundValue;
        }

        private decimal FixedDrawer(decimal val, GUIContent label, Func<GUIContent, bool> callNextDrawer)
        {
            if (IsRadius)
            {
                label.text = "Radius";
                value = DrawRadiusSlider(val, label);
            }
            else if(IsMoveSpeed && moveSpeed != null)
            {
                if (IsEffector)
                {
                    value = EditorGUILayout.IntSlider(new GUIContent("Buff %"), (int)val, 0, 100);
                }
                else
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
                            value = (decimal)MoveSpeedSelector.Speed;
                        };
                    }
                
                    EditorGUILayout.EndHorizontal();
                }
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

                if (GUILayout.Button($"Add | ({value})"))
                {
                    if (newBinary.Length < 8)
                    {
                        newBinary += '0';
                    }
                }
                
                value = Convert.ToInt32(newBinary, 2);
            }
            else if(IsRicochet)
            {
                if (value < 1_000_000_00)
                {
                    value = 1_000_000_00;
                }
                
                var binary = (int)value;
                var percentValue = binary / 100000 % 1000;
                var radius = (binary / 100 % 1000) / 10m;
                var ricochet = binary % 100;

                EditorGUILayout.BeginVertical();

                percentValue = EditorGUILayout.IntSlider(new GUIContent("Decrease %"), percentValue, 0, 100);
                radius = DrawRadiusSlider(radius, new GUIContent("Radius"));
                ricochet = EditorGUILayout.IntSlider(new GUIContent("Ricochet"), ricochet, 1, 10);
                
                EditorGUILayout.EndVertical();

                var newBinary = 1_000_000_00;
                newBinary += percentValue * 100000;
                newBinary += ((int)(radius * 10)) * 100;
                newBinary += ricochet;
                value = newBinary;
            }
            else
            {
                if (IsEffector && !IsHealth)
                {
                    value = EditorGUILayout.IntSlider(new GUIContent(scope.Contains("Potion") ? "Damage/Sec" : "Buff %"), (int)val, 0, 100);
                }
                else
                {
                    if (IsEffector)
                    {
                        label.text = "Duration";
                    }
                    else
                    {
                        label.text = GetType().Name.Replace("GP", string.Empty);
                    }
                    
                    value = (decimal)EditorGUILayout.FloatField(label, (float)val);
                }
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