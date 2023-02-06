using System.Text.RegularExpressions;
using Core.Extensions;
using GameCore.Attributes;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace Attributes.Editor.Drawers
{
    public partial class ColoredFieldPropertyDrawer
    {
        private string prefabPath;
        private int lastModifiedPropCount;
        
        protected override void Initialize()
        {
            base.Initialize();

            var parent = Property.Parent;
            needBack = parent?.GetAttribute<ColoredFieldAttribute>() == null;
            heartTexture = Resources.Load<Texture2D>($"ColoredField/heart");
            blueHeartTexture = Resources.Load<Texture2D>($"ColoredField/blue_heart");

            style = new GUIStyle(SirenixGUIStyles.BoxContainer);

            if (needBack)
            {
                prefabPath = Property.Info.GetMemberInfo().Name;
                realTexture = Resources.Load<Texture2D>($"ColoredField/background{Attribute.myIndex % 4}");

                if (Attribute.colorData.isDark)
                {
                    var colors = realTexture.GetPixels();
                    var newTexture = new Texture2D (realTexture.width, realTexture.height, TextureFormat.RGBA32, false);

                    for (int i = 0; i < colors.Length; i++)
                    {
                        colors[i].r = 1.3f - colors[i].r;
                        colors[i].g = 1.3f - colors[i].g;
                        colors[i].b = 1.3f - colors[i].b;
                    }
                    
                    newTexture.SetPixels(colors);
                    newTexture.Apply();
                    realTexture = newTexture;
                }
                
                texture = new Texture2D (realTexture.width, realTexture.height, TextureFormat.RGBA32, false);
                lastTexture = new Texture2D (realTexture.width, realTexture.height, TextureFormat.RGBA32, false);
                Graphics.CopyTexture(realTexture, 0, 0, 0, 0, realTexture.width, realTexture.height, texture, 0, 0, 0 , 0);
                Graphics.CopyTexture(realTexture, 0, 0, 0, 0, realTexture.width, realTexture.height, lastTexture, 0, 0, 0 , 0);
            
                lastWidth = lastTexture.width;
                lastHeight = lastTexture.height;
                labelWidth = GUIHelper.BetterLabelWidth - 4f;
            }
            else if (parent != null)
            {
                var name = Property.Info.GetMemberInfo().Name;
                prefabPath = $"{Property.PrefabModificationPath}.{name}";
                style.margin = new RectOffset(10, 10, 10, 10);
            }

            OnPropPrefabUpdated();
            var colorData = Attribute.colorData;
            normalColor = colorData.isDark ? Color.white.CloneAndChangeBrightness(0.8f) : Color.white.CloneAndChangeBrightness(0.2f);
            hoverColor = colorData.isDark ? Color.white : Color.black;
            labelName = Property.Info.GetMemberInfo().Name.SplitPascalCase();
        }

        private void OnPropPrefabUpdated()
        {
            var propertyModifications = PrefabUtility.GetPropertyModifications(Selection.activeGameObject);
            
            if (propertyModifications == null || propertyModifications.Length == lastModifiedPropCount) return;

            for (int i = 0; i < propertyModifications.Length; i++)
            {
                var prop = propertyModifications[i];
                var isMatch = Regex.Match(prop.propertyPath, @$"^({prefabPath})\.(\w+)$");
            
                if (isMatch.Captures.Count != 0)
                {
                    Attribute.isModified = true;
                    break;
                }
            }
            
            lastModifiedPropCount = propertyModifications.Length;
            
        }
    }
}