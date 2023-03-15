using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityToolbarExtender;
using static UnityEditor.AssetDatabase;

namespace BeatRoyale
{
    [InitializeOnLoad]
    public static class RightToolBar
    {
        static RightToolBar()
        {
            ToolbarExtender.RightToolbarGUI.Add(OnToolbarRightGUI);
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
                PopupWindow.Show(GUILayoutUtility.GetLastRect(), new Popup());
            }
        }

        public class Popup : PopupWindowContent
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
    }
}