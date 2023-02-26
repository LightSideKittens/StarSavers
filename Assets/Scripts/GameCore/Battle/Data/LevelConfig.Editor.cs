using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEditor;

#if UNITY_EDITOR
namespace Battle.Data
{
    public partial class LevelConfig
    {
        [InfoBox("Config is invalid. Check config name.", InfoMessageType.Error, "$" + nameof(IsValid))]
        private bool isInited;
        private bool isSubscribed;
        private string entityScope;
        private int currentLevel;
        private bool levelParsed;
        private bool IsValid => !string.IsNullOrEmpty(entityScope) && levelParsed;

        private void OnValidate()
        {
            if (!isSubscribed)
            {
                AssemblyReloadEvents.beforeAssemblyReload += OnBeforeAssemblyReload;
                isSubscribed = true;
            }

            if (!IsValid)
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
            var scopeName = split[0];
            entityScope = GameScopes.GetEntityNameByScopeName(scopeName);
            levelParsed = int.TryParse(split[^1], out currentLevel);
            
            if (!IsValid)
            {
                return;
            }

            isInited = true;
            var editorLevels = EditorLevels.Config.LevelsNames;
            EditorLevels.LoadOnNextAccess();
            ChangedLevels.LoadOnNextAccess();
            if (!editorLevels.Contains(name))
            {
                editorLevels.Add(name);
                TryUpdateChangedLevels();
                EditorLevels.Editor_SaveAsDefault();
                AssetDatabase.Refresh();
            }
        }

        private void TryUpdateChangedLevels()
        {
            var changedLevels = ChangedLevels.Config.Levels;
            if (changedLevels.TryGetValue(entityScope, out var level))
            {
                if (level > currentLevel)
                {
                    changedLevels[entityScope] = currentLevel;
                    ChangedLevels.Editor_SaveAsDefault();
                    AssetDatabase.Refresh();
                }
            }
            else
            {
                changedLevels.Add(entityScope, currentLevel);
                ChangedLevels.Editor_SaveAsDefault();
                AssetDatabase.Refresh();
            }
        }

        [OnInspectorGUI]
        private void OnInspectorGUI()
        {
            if (!IsValid)
            {
                return;
            }
            
            var scopeHashSet = new HashSet<string>();
            var splitedName = name.Split('_');

            if (UpgradesByScope.Count > 0)
            {
                if (splitedName[^1] == "1")
                {
                    var upgrade = UpgradesByScope[0];
                    isError = !upgrade.Scope.Contains(splitedName[0]);

                    if (!isError)
                    {
                        for (int j = 0; j < upgrade.Properties.Count; j++)
                        {
                            var prop = upgrade.Properties[j];

                            if (prop.Fixed == 0)
                            {
                                isError = true;
                                break;
                            }
                        }
                    }
                }
            }
            
            for (int i = 0; i < UpgradesByScope.Count; i++)
            {
                var step = UpgradesByScope[i];
                step.isError = !scopeHashSet.Add(step.Scope);
                var props = step.Properties;

                for (int j = 0; j < props.Count; j++)
                {
                    var prop = props[j];
                    var needHideFixed = !GameScopes.EntityScopesSet.Contains(step.Scope);

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
