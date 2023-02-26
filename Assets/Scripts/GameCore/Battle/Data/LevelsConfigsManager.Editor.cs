#if UNITY_EDITOR
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEditor;
using UnityEngine;

namespace Battle.Data
{
    public partial class LevelsConfigsManager
    {
        [OdinSerialize] private Object levelsFolder;
        [OdinSerialize, ReadOnly] private List<string> paths = new();
        private string levelsFolderPath;
        
        [OnInspectorInit]
        private void OnInspectorInit()
        {
            hasError = true;
        }

        [OnInspectorGUI]
        private void OnInspectorGUI()
        {
            if (hasError)
            {
                if (levelsFolder != null)
                {
                    Editor_Init();
                }
            }
        }

        private void Editor_Init()
        {
            levelsFolderPath = AssetDatabase.GetAssetPath(levelsFolder);
            paths.Clear();
            levels.Clear();
            InitFolders();
            hasError = false;

            for (int i = 0; i < paths.Count; i++)
            {
                var path = paths[i];
                var guids = AssetDatabase.FindAssets(string.Empty, new[] {path});
                var scope = path.Replace(levelsFolderPath, "Global");
                var error = !GameScopes.EntityScopesSet.Contains(scope);
                hasError |= error;
                var level = new Levels()
                {
                    scope = scope,
                    isError = error
                };

                for (int j = 0; j < guids.Length; j++)
                {
                    var levelConfig =
                        AssetDatabase.LoadAssetAtPath<LevelConfig>(AssetDatabase.GUIDToAssetPath(guids[j]));
                    level.levels.Add(levelConfig);
                }

                levels.Add(level);
            }
        }

        private void InitFolders()
        {
            var folders = AssetDatabase.GetSubFolders(levelsFolderPath);

            foreach (var folder in folders)
            {
                Recursive(folder);
            }
        }

        private void Recursive(string folder)
        {
            var folders = AssetDatabase.GetSubFolders(folder);

            foreach (var fld in folders)
            {
                Recursive(fld);
            }

            if (folders.Length == 0)
            {
                paths.Add(folder);
            }
        }
    }
}
#endif