#if UNITY_EDITOR
using System.Collections.Generic;
using Battle.Data.GameProperty;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEditor;
using UnityEngine;

namespace Battle.Data
{
    public partial class LevelsConfigsManager
    {
        private readonly HashSet<string> entitesNames = new();

        public HashSet<string> EntitesNames
        {
            get
            {
                for (int i = 0; i < levelsContainers.Count; i++)
                {
                    entitesNames.Add(levelsContainers[i].entityName);
                }

                return entitesNames;
            }
        }
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

        public static LevelsConfigsManager Editor_GetInstance()
        {
            var guid = AssetDatabase.FindAssets("t: LevelsConfigsManager");
            return AssetDatabase.LoadAssetAtPath<LevelsConfigsManager>(AssetDatabase.GUIDToAssetPath(guid[0]));
        }
        
        public static void Editor_RecomputeAllLevels()
        {
            Editor_GetInstance().RecomputeAllLevels();
        }
    }
}
#endif