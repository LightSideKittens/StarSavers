using DG.DemiEditor;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityToolbarExtender;
using static EditorUtils;
using static UnityEditor.AssetDatabase;

namespace BeatRoyale
{
    [InitializeOnLoad]
    public static partial class ToolBar
    {
        public static readonly GUIStyle GreenButtonStyle = new ();
        public static readonly GUIStyle RedButtonStyle = new();
        private static readonly Color green = new(0.49f, 1f, 0.42f);
        private static readonly Color red = new(1f, 0.53f, 0.48f);
        
        static ToolBar()
        {
            ToolbarExtender.LeftToolbarGUI.Add(OnToolbarLeftGUI);
            ToolbarExtender.RightToolbarGUI.Add(OnToolbarRightGUI);
            ConfigureToggleButtonStyle();
        }

        private static void OnToolbarRightGUI()
        {
            if (GUILayout.Button("Launcher", GUILayout.MaxWidth(100)))
            {
                EditorSceneManager.OpenScene("Assets/Scenes/Launcher.unity");
            }
            
            if (GUILayout.Button("Battle", GUILayout.MaxWidth(100)))
            {
                EditorSceneManager.OpenScene("Assets/Scenes/Battle.unity");
            }

            if (GUILayout.Button("Select...", GUILayout.MaxWidth(100)))
            {
                PopupWindow.Show(GUILayoutUtility.GetLastRect(), new NavigationPopup());
            }
        }

        public class NavigationPopup : PopupWindowContent
        {
            public override void OnGUI(Rect rect)
            {
                DrawButton("Cards Config", "t: Cards");
                DrawButton("Units Config", "t: Units");
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
            ConfigureToggleButtonStyle(GreenButtonStyle, green);
            ConfigureToggleButtonStyle(RedButtonStyle, red);
        }

        private static void ConfigureToggleButtonStyle(GUIStyle style, Color color)
        {
            style.alignment = TextAnchor.MiddleCenter;
            style.margin = new RectOffset(10, 10, 0, 0);
            style.richText = true;
            var textColor = new Color(0.17f, 0.17f, 0.17f);

            var normal = style.normal;
            normal.textColor = textColor;
            SetBackground(normal, GetTextureByColor(color));

            var hover = style.hover;
            hover.textColor = textColor;
            SetBackground(hover, GetTextureByColor(color.CloneAndChangeBrightness(0.9f)));

            var active = style.active;
            active.textColor = textColor;
            SetBackground(active, GetTextureByColor(color.CloneAndChangeBrightness(0.8f)));

            void SetBackground(GUIStyleState state, Texture2D texture)
            {
                if (state.background == null)
                {
                    state.background = texture;
                }
            }
        }
    }
}