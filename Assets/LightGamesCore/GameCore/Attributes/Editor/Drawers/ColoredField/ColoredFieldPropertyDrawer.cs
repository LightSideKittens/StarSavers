using System;
using System.Collections.Generic;
using Core.Extensions;
using GameCore.Attributes;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace Attributes.Editor.Drawers
{
    public partial class ColoredFieldPropertyDrawer : OdinGroupDrawer<ColoredFieldAttribute>
    {
        private bool isOpen;
        private GUIStyle style;
        private Texture2D blueHeartTexture;
        private Texture2D heartTexture;
        private Texture2D lastTexture;
        private Texture2D texture;
        private Texture2D realTexture;
        private string labelName;
        private float labelWidth;
        
        private int widthChankCount = 1;
        private int heightChankCount = 1;
        
        private int lastWidth = 0;
        private int lastHeight = 0;
        private bool needBack;
        private Color normalColor;
        private Color hoverColor;

        protected override void DrawPropertyLayout(GUIContent label)
        {
            if (Property.Children[0].ValueEntry.WeakSmartValue != null)
            {
                if (needBack)
                {
                    SirenixGUIStyles.Foldout.normal.textColor = normalColor; 
                    style.normal.background = lastTexture;
                    BaseDraw(label);
                    DrawBack();
                }
                else
                {
                    style.normal.background = EditorUtils.GetTextureByColor(Color.gray * 1.2f);
                    BaseDraw(label);
                }
            }
            else
            {
                for (int i = 0; i < Property.Children.Count; i++)
                {
                    Property.Children[i].Draw();
                }
            }
        }

        private void ResizeAndCopyTexture()
        {
            lastWidth = realTexture.width * widthChankCount;
            lastHeight = realTexture.height * heightChankCount;
            
            lastTexture.Reinitialize(lastWidth, lastHeight, TextureFormat.RGBA32, false);
            lastTexture.Apply();
            
            var newTexture = new Texture2D (texture.width, texture.height, TextureFormat.RGBA32, false);
            Graphics.CopyTexture(texture, 0, 0, 0, 0, texture.width, texture.height, newTexture, 0, 0, 0 , 0);

            texture.Reinitialize(lastWidth, lastHeight, TextureFormat.RGBA32, false);
            texture.Apply();
            
            Graphics.CopyTexture(newTexture, 0, 0, 0, 0, newTexture.width, newTexture.height, texture, 0, 0, 0 , 0);
        }
        
        public bool Foldout(bool isVisible, GUIContent label, GUIStyle style = null)
        {
            float fieldWidth = EditorGUIUtility.fieldWidth;
            EditorGUIUtility.fieldWidth = 10f;
            Rect controlRect = EditorGUILayout.GetControlRect(false);
            EditorGUIUtility.fieldWidth = fieldWidth;
            return Foldout(controlRect, isVisible, label, style);
        }
        
        public bool Foldout(Rect rect, bool isVisible, GUIContent label, GUIStyle style = null)
        {
            style = style ?? SirenixGUIStyles.Foldout;
            style.normal.textColor = normalColor;
            style.focused.textColor = normalColor;
            style.active.textColor = normalColor;
            style.hover.textColor = normalColor;
            UnityEngine.EventType type = Event.current.type;
            bool flag = false;
            if (type != UnityEngine.EventType.Layout)
                flag = rect.Contains(Event.current.mousePosition);

            if (flag)
            {
                GUIHelper.PushLabelColor(hoverColor);
            }
            else if (isOpen)
            {
                GUIHelper.PushLabelColor(normalColor);
            }
            
            if (flag && type == UnityEngine.EventType.MouseMove)
                GUIHelper.RequestRepaint();
            if (type == UnityEngine.EventType.MouseDown & flag && Event.current.button == 0)
            {
                isVisible = !isVisible;
                GUIHelper.RequestRepaint();
                GUIHelper.PushGUIEnabled(true);
                Event.current.Use();
                GUIHelper.PopGUIEnabled();
                GUIHelper.RemoveFocusControl();
            }
            isVisible = EditorGUI.Foldout(rect, isVisible, label, style);
            if (flag || isOpen)
                GUIHelper.PopLabelColor();
            return isVisible;
        }

        private Rect BeginBoxHeader()
        {
            GUILayout.Space(-3f);
            Rect rect = EditorGUILayout.BeginHorizontal(SirenixGUIStyles.BoxHeaderStyle, (GUILayoutOption[]) GUILayoutOptions.ExpandWidth());
            if (Event.current.type == UnityEngine.EventType.Repaint)
            {
                rect.x -= 3f;
                rect.width += 6f;
                
                UnityEngine.EventType type = Event.current.type;
                bool flag = false;
                if (type != UnityEngine.EventType.Layout)
                    flag = rect.Contains(Event.current.mousePosition);
                
                var color = Attribute.colorData.color;
                if (flag)
                {
                    color = color.CloneAndChangeSaturation(1.2f);
                    color = color.CloneAndChangeBrightness(1.2f);
                    
                    GUIHelper.PushColor(color);
                }
                else
                {
                    GUIHelper.PushColor(color);
                }

                GUI.DrawTexture(rect, (Texture) Texture2D.whiteTexture);
                GUIHelper.PopColor();
                SirenixEditorGUI.DrawBorders(rect, 0, 0, 0, 1, new Color(0.0f, 0.0f, 0.0f, 0.4f));
            }
            GUIHelper.PushLabelWidth(GUIHelper.BetterLabelWidth - 4f);
            return rect;
        }
    }
}