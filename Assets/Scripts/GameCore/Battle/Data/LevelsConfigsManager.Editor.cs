#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
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
        private bool isInited;
        
        [OnInspectorInit]
        private void OnInspectorInit()
        {
            hasError = true;
        }

        [OnInspectorDispose]
        private void OnInspectorDispose()
        {
            isInited = false;
        }
        
        [OnInspectorGUI]
        private void OnInspectorGUI()
        {
            if (hasError)
            {
                if (levelsFolder != null && !isInited)
                {
                    Editor_Init();
                }
            }
        }

        private void Editor_Init()
        {
            isInited = true;
            levelsFolderPath = AssetDatabase.GetAssetPath(levelsFolder);
            var editorLevels = EditorLevels.Config.LevelsNames;
            
            editorLevels.Clear();
            levelsContainers.Clear();
            paths.Clear();
            
            InitFolders();
            hasError = false;

            for (int i = 0; i < paths.Count; i++)
            {
                var path = paths[i];
                var guids = AssetDatabase.FindAssets(string.Empty, new[] {path});
                var levelsContainer = new LevelsContainer();

                var lastLevel = 0;
                
                for (int j = 0; j < guids.Length; j++)
                {
                    var levelConfig = AssetDatabase.LoadAssetAtPath<LevelConfig>(AssetDatabase.GUIDToAssetPath(guids[j]));
                    hasError |= levelConfig.IsInvalid;
                    hasError |= levelsContainer.isMissedLevel;

                    if (!hasError)
                    {
                        var currentLevel = levelConfig.CurrentLevel;
                        levelsContainer.entityName = levelConfig.EntityName;
                        levelsContainer.isMissedLevel = currentLevel - lastLevel > 1;
                        levelsContainer.missedLevel = currentLevel - 1;
                        lastLevel = currentLevel;

                        levelsContainer.levels.Add(levelConfig);
                        editorLevels.Add(levelConfig.name);
                        levelsContainers.Add(levelsContainer);
                    }
                }
            }

            if (!hasError)
            {
                EditorLevels.Save();
                AssetDatabase.Refresh();
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