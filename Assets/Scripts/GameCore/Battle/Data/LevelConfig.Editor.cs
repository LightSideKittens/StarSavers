#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEditor;

namespace Battle.Data
{
    public partial class LevelConfig
    {
        private bool isInited;
        private bool isSubscribed;
        private string entityName;
        private int currentLevel;
        private bool levelParsed;
        private bool isError;
        private bool isChangedLevels;
        private bool isChangedEditorLevels;
        public bool IsInvalid => string.IsNullOrEmpty(entityName) || !levelParsed;
        public int CurrentLevel => currentLevel;
        public string EntityName => entityName;

        private void OnValidate()
        {
            if (!isSubscribed)
            {
                AssemblyReloadEvents.beforeAssemblyReload += OnBeforeAssemblyReload;
                isSubscribed = true;
            }

            if (IsInvalid)
            {
                return;
            }
            
            if (isInited)
            {
                TryUpdateChangedLevels();
            }
        }

        private void OnBeforeAssemblyReload()
        { 
            isInited = false;
            AssemblyReloadEvents.beforeAssemblyReload -= OnBeforeAssemblyReload;
            isSubscribed = false;
        }

        [OnInspectorInit]
        private void OnInspectorInit()
        {
            var split = name.Split('_');
            var configEntityName = split[0];
            entityName = GameScopes.IsEntityName(configEntityName) ? configEntityName : null;
            levelParsed = int.TryParse(split[^1], out currentLevel);
            
            if (IsInvalid)
            {
                return;
            }

            isInited = true;
            TryUpdateChangedLevels();
        }

        private void TryUpdateChangedLevels()
        {
            var changedLevels = ChangedLevels.Config.Levels;

            if (changedLevels.TryGetValue(entityName, out var level))
            {
                if (level > currentLevel)
                {
                    changedLevels[entityName] = currentLevel;
                    isChangedLevels = true;
                }
            }
            else
            {
                changedLevels.Add(entityName, currentLevel);
                isChangedLevels = true;
            }
            
            if (!EditorLevels.Config.LevelsNames.Contains(name))
            {
                EditorLevels.Config.LevelsNames.Add(name);
                changedLevels.TryAdd(entityName, currentLevel);
                isChangedLevels = true;
                isChangedEditorLevels = true;
            }
        }

        [OnInspectorDispose]
        private void OnInspectorDispose()
        {
            if (isChangedLevels)
            {
                isChangedLevels = false;
                ChangedLevels.Save();
                AssetDatabase.Refresh();
            }

            if (isChangedEditorLevels)
            {
                isChangedEditorLevels = false;
                EditorLevels.Save();
                AssetDatabase.Refresh();
            }
        }

        [OnInspectorGUI]
        private void OnInspectorGUI()
        {
            if (IsInvalid)
            {
                return;
            }
            
            var splitedName = name.Split('_');

            if (splitedName[^1] == "1")
            {
                if (UpgradesByScope.Count > 0)
                {
                    var upgrade = UpgradesByScope[0];
                    isError = !GameScopes.TryGetEntityNameFromScope(upgrade.Scope, out var entityName) ||
                        entityName != splitedName[0];

                    if (!isError)
                    {
                        var props = upgrade.Properties;

                        if (props.Count != 0)
                        {
                            for (int j = 0; j < props.Count; j++)
                            {
                                var prop = props[j];

                                if (prop.Fixed == 0)
                                {
                                    isError = true;

                                    break;
                                }
                            }
                        }
                        else
                        {
                            isError = true;
                        }
                    }
                }
                else
                {
                    isError = true;
                }
            }
            
            var scopeHashSet = new HashSet<string>();
            for (int i = 0; i < UpgradesByScope.Count; i++)
            {
                var step = UpgradesByScope[i];
                var scope = step.Scope;
                var isEntity = GameScopes.TryGetEntityNameFromScope(scope, out var entityName);
                step.isError = !scopeHashSet.Add(scope);
                var props = step.Properties;
                step.entityName = entityName;

                for (int j = 0; j < props.Count; j++)
                {
                    var prop = props[j];
                    var needHideFixed = !isEntity;

                    if (needHideFixed)
                    {
                        prop.Fixed = 0;
                    }
                    
                    prop.needHideFixed = needHideFixed;
                }
            }

            if (UpgradesByScope.Count > 1)
            {
                UpgradesByScope.Sort((a, b) =>
                {
                    var bLength = b.Scope.Split('/').Length;
                    var aLength = a.Scope.Split('/').Length;
                    return Math.Sign(bLength - aLength);
                });
            }
        }
    }
}
#endif
