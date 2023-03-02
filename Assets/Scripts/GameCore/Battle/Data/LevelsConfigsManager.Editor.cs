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
        private readonly List<string> paths = new();
        private string levelsFolderPath;
        private string assetPath;
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
                if (!isInited)
                {
                    Editor_Init();
                    Save();
                }
            }
        }

        private void Editor_Init()
        {
            isInited = true;
            assetPath = AssetDatabase.GetAssetPath(this);
            levelsFolderPath = assetPath.Replace($"/{name}.asset", string.Empty);
            levelsContainers.Clear();
            paths.Clear();
            
            InitFolders();
            hasError = false;

            for (int i = 0; i < paths.Count; i++)
            {
                var path = paths[i];
                var guids = AssetDatabase.FindAssets("t: LevelConfig", new[] {path});

                if (guids.Length > 0)
                {
                    var levelsContainer = new LevelsContainer();
                    var lastLevel = 0;

                    for (int j = 0; j < guids.Length; j++)
                    {
                        var levelConfig = AssetDatabase.LoadAssetAtPath<LevelConfig>(AssetDatabase.GUIDToAssetPath(guids[j]));
                        hasError |= levelConfig.IsInvalid;
                        hasError |= levelsContainer.isMissedLevel;
                    
                        levelsContainer.entityName = levelConfig.EntityName;
                        var currentLevel = levelConfig.CurrentLevel;

                        if (currentLevel - lastLevel > 1)
                        {
                            levelsContainer.isMissedLevel = currentLevel - lastLevel > 1;
                            levelsContainer.missedLevel = currentLevel - 1;
                        }

                        if (levelConfig.IsInvalid)
                        {
                            levelsContainer.isLevelError |= levelConfig.IsInvalid;
                            levelsContainer.levelErrorName = levelConfig.name;
                        }

                        lastLevel = currentLevel;

                        levelsContainer.levels.Add(levelConfig);
                    }
                
                    levelsContainers.Add(levelsContainer);
                }
            }
        }

        private void Save()
        {
            AssetDatabase.ForceReserializeAssets(new []{assetPath});
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