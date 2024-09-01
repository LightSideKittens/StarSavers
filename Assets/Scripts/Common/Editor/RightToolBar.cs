using DG.DemiEditor;
using UnityEditor;
using UnityEngine;
using UnityToolbarExtender;
using static EditorUtils;
using static UnityEditor.AssetDatabase;

namespace StarSavers
{
    [InitializeOnLoad]
    public static partial class ToolBar
    {
        public static GUIStyle GreenButtonStyle;
        public static GUIStyle RedButtonStyle;
        private static readonly Color green = new(0.49f, 1f, 0.42f);
        private static readonly Color red = new(1f, 0.53f, 0.48f);
        
        static ToolBar()
        {
            ToolbarExtender.LeftToolbarGUI.Add(ConfigureToggleButtonStyle);
            ToolbarExtender.RightToolbarGUI.Add(ConfigureToggleButtonStyle);
            ToolbarExtender.LeftToolbarGUI.Add(OnToolbarLeftGUI);
            ToolbarExtender.RightToolbarGUI.Add(OnToolbarRightGUI);
        }

        private static void OnToolbarRightGUI()
        {
            var rect = GUILayoutUtility.GetRect(new GUIContent("Select..."), GUI.skin.button, GUILayout.MaxWidth(100));
            if (GUI.Button(rect, "Select..."))
            {
                PopupWindow.Show(rect, new NavigationPopup());
            }
        }

        public class NavigationPopup : PopupWindowContent
        {
            public override void OnGUI(Rect rect)
            {
                GUILayout.Label("Scenes");
                DrawButton("Launcher", "t: Scene Launcher");
                DrawButton("Battle", "t: Scene Battle");
                
                GUILayout.Label("Configs");
                DrawButton("Heroes Config", "t: UnitsById Heroes");
                DrawButton("Enemies Config", "t: UnitsById Enemies");
                DrawButton("Effectors Config", "t: Effectors");
                DrawButton("Levels Configs Manager", "t: LevelsConfigsManager");
            }

            private void DrawButton(string name, string filter)
            {
                if (GUILayout.Button(name, GUILayout.MaxWidth(200)))
                {
                    var guids = FindAssets(filter);
                    var obj = LoadMainAssetAtPath(GUIDToAssetPath(guids[0]));
                    EditorGUIUtility.PingObject(obj);
                    Selection.activeObject = obj;
                    editorWindow.Close();
                }
            }
        }

        private static void ConfigureToggleButtonStyle()
        {
            if (GreenButtonStyle == null)
            {
                GreenButtonStyle = new GUIStyle(GUI.skin.button);
                RedButtonStyle = new GUIStyle(GreenButtonStyle);
                ConfigureToggleButtonStyle(GreenButtonStyle, green);
                ConfigureToggleButtonStyle(RedButtonStyle, red);
            }
        }

        private static void ConfigureToggleButtonStyle(GUIStyle style, Color color)
        {
            style.alignment = TextAnchor.MiddleCenter;
            style.margin = new RectOffset(10, 10, 0, 0);
            style.richText = true;
            var textColor = new Color(0.17f, 0.17f, 0.17f);
            
            style.Add(textColor);
            var back = GetTextureByColor(color);
            var back2 = GetTextureByColor(color.CloneAndChangeBrightness(0.9f));
            var back3 = GetTextureByColor(color.CloneAndChangeBrightness(0.75f));

            style.Background(back, back3, back2);
        }
    }
}