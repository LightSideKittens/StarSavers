#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
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
        private bool isFirstLevelError;
        private bool hasScopeError;
        private bool hasPropTypeError;
        private bool isChangedLevels;
        private string[] splitedName;
        public bool IsInvalidName => string.IsNullOrEmpty(entityName) || !levelParsed;
        public bool IsInvalid => isFirstLevelError || hasScopeError || hasPropTypeError || IsInvalidName;
        public int CurrentLevel => currentLevel;
        public string EntityName => entityName;

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
            levelParsed = int.TryParse(splitedName[^1], out currentLevel);
            
            if (IsInvalidName)
            {
                return;
            }

            for (int i = 0; i < UpgradesByScope.Count; i++)
            {
                UpgradesByScope[i].ScopeChanged += OnScopeChanged;
            }

            isInited = true;
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

        [OnInspectorDispose]
        private void OnInspectorDispose()
        {
            if (isChangedLevels)
            {
                isChangedLevels = false;
                ChangedLevels.Save();
                AssetDatabase.Refresh();
            }
            
            OnScopeChanged();

            for (int i = 0; i < UpgradesByScope.Count; i++)
            {
                UpgradesByScope[i].ScopeChanged -= OnScopeChanged;
            }
        }

        [OnInspectorGUI]
        private void OnInspectorGUI()
        {
            if (IsInvalidName)
            {
                return;
            }

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

                                if (prop.Fixed == 0)
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

        private void OnScopeChanged()
        {
            hasScopeError = false;
            hasPropTypeError = false;
            var scopeHashSet = new HashSet<string>();
            var propTypeHashSet = new HashSet<Type>();
            
            for (int i = 0; i < UpgradesByScope.Count; i++)
            {
                var step = UpgradesByScope[i];
                propTypeHashSet.Clear();
                var scope = step.Scope;
                var isEntity = GameScopes.TryGetEntityNameFromScope(scope, out var entityName);
                var error = !scopeHashSet.Add(scope);
                step.isError = error;
                hasScopeError |= error;
                var props = step.Properties;
                step.entityName = entityName;

                for (int j = 0; j < props.Count; j++)
                {
                    var prop = props[j];
                    var propError = !propTypeHashSet.Add(prop.GetType());
                    prop.isError = propError;
                    hasPropTypeError |= propError;
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
                    var bLength = b.Scope.Count(t=> t == '/');
                    var aLength = a.Scope.Count(t=> t == '/');
                    return Math.Sign(bLength - aLength);
                });
            }
        }
    }
}
#endif
