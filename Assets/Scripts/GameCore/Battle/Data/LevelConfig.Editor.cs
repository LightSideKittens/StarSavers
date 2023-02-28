#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using Battle.Data.GameProperty;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace Battle.Data
{
    public partial class LevelConfig
    {
        [HideInInspector] public string entityName;
        [HideInInspector] public int currentLevel;
        [HideInInspector] public bool isFirstLevelError;
        [HideInInspector] public bool hasScopeError;
        [HideInInspector] public bool hasEmptyScopeError;
        private bool isInited;
        private bool isSubscribed;
        private bool isChangedLevels;
        private string[] splitedName;
        public bool IsInvalidName => string.IsNullOrEmpty(entityName) || currentLevel == 0;
        public bool IsInvalid => isFirstLevelError || hasScopeError || hasEmptyScopeError || IsInvalidName;

        private void OnValidate()
        {
            if (!isSubscribed)
            {
                AssemblyReloadEvents.beforeAssemblyReload += OnBeforeAssemblyReload;
                isSubscribed = true;
            }

            if (IsInvalidName)
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
            splitedName = name.Split('_');
            var configEntityName = splitedName[0];
            entityName = GameScopes.IsEntityName(configEntityName) ? configEntityName : null;
            int.TryParse(splitedName[^1], out currentLevel);
            
            if (IsInvalidName)
            {
                return;
            }

            OnUpgradeStepsChanged();
            isInited = true;
        }
        
        [OnInspectorGUI]
        private void OnInspectorGUI()
        {
            if (IsInvalidName)
            {
                return;
            }

            OnFirstLevel();
            
            for (int i = 0; i < UpgradesByScope.Count; i++)
            {
                UpgradesByScope[i].OnGUI();
                UpgradesByScope[i].level = currentLevel;
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
            
            for (int i = 0; i < UpgradesByScope.Count; i++)
            {
                UpgradesByScope[i].ScopeChanged -= OnScopeChanged;
            }
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
        }

        private void OnFirstLevel()
        {
            if (currentLevel == 1)
            {
                if (UpgradesByScope.Count > 0)
                {
                    var upgrade = UpgradesByScope[0];
                    isFirstLevelError = !GameScopes.TryGetEntityNameFromScope(upgrade.Scope, out var entityName) ||
                        entityName != splitedName[0];

                    if (!isFirstLevelError)
                    {
                        var props = upgrade.Properties;

                        if (props.Count != 0)
                        {
                            for (int j = 0; j < props.Count; j++)
                            {
                                var prop = props[j];

                                if (prop.value == 0)
                                {
                                    isFirstLevelError = true;

                                    break;
                                }
                            }
                        }
                        else
                        {
                            isFirstLevelError = true;
                        }
                    }
                }
                else
                {
                    isFirstLevelError = true;
                }
            }
        }

        private void OnUpgradeStepsChanged()
        {
            for (int i = 0; i < UpgradesByScope.Count; i++)
            {
                UpgradesByScope[i].ScopeChanged -= OnScopeChanged;
                UpgradesByScope[i].ScopeChanged += OnScopeChanged;
            }

            OnScopeChanged();
        }

        public static void ReanalyzeScope(GamePropertiesByScope step)
        {
            var props = step.Properties;
            var isEntity = GameScopes.TryGetEntityNameFromScope(step.Scope, out var entityName);
            step.entityName = entityName;
            
            for (int j = 0; j < props.Count; j++)
            {
                var prop = props[j];
                var needHideFixed = !isEntity;

                if (needHideFixed)
                {
                    prop.value = 0;
                }
                    
                prop.needHideFixed = needHideFixed;
            }
        }
        
        private void OnScopeChanged()
        {
            hasScopeError = false;
            hasEmptyScopeError = false;
            var scopeHashSet = new HashSet<string>();

            for (int i = 0; i < UpgradesByScope.Count; i++)
            {
                var step = UpgradesByScope[i];
                var scope = step.Scope;
                var error = !scopeHashSet.Add(scope);
                step.isMultipleIdenticalScopeError = error;
                hasScopeError |= error;
                var props = step.Properties;
                hasEmptyScopeError |= props.Count == 0;
                step.isEmptyScopeError = props.Count == 0;
                ReanalyzeScope(step);
            }
            
            if (UpgradesByScope.Count > 1)
            {
                UpgradesByScope.Sort((a, b) =>
                {
                    var bLength = b.Scope.Count(t=> t == '/');
                    var aLength = a.Scope.Count(t=> t == '/');
                    return Math.Sign(bLength - aLength);
                });
            }
        }
    }
}
#endif
