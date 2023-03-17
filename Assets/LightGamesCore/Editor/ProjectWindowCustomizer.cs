#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEditor.Toolbars;
using UnityEngine;
using static UnityEditor.AssetDatabase;

namespace Fishcoin
{
    [InitializeOnLoad]
    public static class ProjectWindowCustomizer
    {
        private static readonly Texture2D overlay = EditorUtils.GetTextureByColor(new Color(0.21f, 0.21f, 0.21f));
        private static readonly Dictionary<string, Texture2D> texturesByGuid = new();

        static ProjectWindowCustomizer()
        {
            EditorApplication.projectWindowItemOnGUI += DrawAssetDetails;

            var toRemove = new List<string>();
            var textureGuidByAssetGuid = EditorData.Config.textureGuidByAssetGuid;
            
            foreach (var textureGuid in textureGuidByAssetGuid)
            {
                var asset = GUIDToAssetPath(textureGuid.Key);

                if (string.IsNullOrEmpty(asset))
                {
                    toRemove.Add(textureGuid.Key);
                    continue;
                }
                
                texturesByGuid.Add(textureGuid.Key, LoadAssetAtPath<Texture2D>(GUIDToAssetPath(textureGuid.Value)));
            }

            for (int i = 0; i < toRemove.Count; i++)
            {
                textureGuidByAssetGuid.Remove(toRemove[i]);
            }
            
            EditorData.Save();
        }

        private static void DrawAssetDetails(string guid, Rect selectionrect)
        {
            var path = GUIDToAssetPath(guid);
            var asset = LoadAssetAtPath<Texture>(path);
            var textStyle = GUIStyle.none;
            textStyle.alignment = TextAnchor.MiddleRight;
            textStyle.normal.textColor = new Color(1f, 1f, 1f, 0.5f);
            GUI.Label(selectionrect, Path.GetExtension(path), textStyle);
            
            if (asset != null || IsValidFolder(path))
            {
                return;
            }

            if (Event.current.alt)
            {
                if (selectionrect.Contains(Event.current.mousePosition))
                {
                    if (Event.current.type == EventType.MouseDown)
                    {
                        PopupWindow.Show(selectionrect, new IconChoosingPopup(guid));
                    }
                    
                    GUI.changed = true;
                    selectionrect.xMax = selectionrect.x + selectionrect.height;
                    DrawOverlay(selectionrect);
                }
            }
            
            if (texturesByGuid.TryGetValue(guid, out var texture))
            {
                selectionrect.xMax = selectionrect.x + selectionrect.height;
                DrawOverlay(selectionrect);
                GUI.DrawTexture(selectionrect, texture, ScaleMode.ScaleToFit);
            }
        }

        private static void DrawOverlay(Rect rect)
        {
            GUI.DrawTexture(rect, overlay);
        }
        
        public class IconChoosingPopup : PopupWindowContent
        {
            private Texture2D selectedTexture;
            private string guid;

            public IconChoosingPopup(string guid)
            {
                this.guid = guid;
            }

            public override void OnClose()
            {
                base.OnClose();

                if (selectedTexture != null)
                {
                    texturesByGuid[guid] = selectedTexture;
                    EditorData.Config.textureGuidByAssetGuid[guid] = AssetPathToGUID(GetAssetPath(selectedTexture));
                    EditorData.Save();
                }
                else
                {
                    texturesByGuid.Remove(guid);
                    EditorData.Config.textureGuidByAssetGuid.Remove(guid);
                    EditorData.Save();
                }
            }

            public override void OnGUI(Rect rect)
            {
                selectedTexture = (Texture2D)EditorGUILayout.ObjectField(selectedTexture, typeof(Texture2D), false);
            }
        }
    }
}
#endif