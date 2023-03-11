#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using Battle.Data.GameProperty;
using Sirenix.OdinInspector;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace Battle.Data
{
    public partial class LevelConfig
    {
        private int currentLevel;
        private bool isFirstLevelError;
        private bool hasScopeError;
        private bool hasEmptyScopeError;
        private bool isInited;
        private bool isSubscribed;
        private bool isChangedLevels { get; set; }
        private string[] splitedName;
        private string configName;
        private bool IsInvalidName => string.IsNullOrEmpty(EntityName) || currentLevel == 0;
        public bool IsInvalid => isFirstLevelError || hasScopeError || hasEmptyScopeError || IsInvalidName;
        public Dictionary<Type, BaseWallet> AddedPrices { get; set; } = new();
        public string EntityName { get; private set; }

        public int CurrentLevel => currentLevel;

        public void OnValidate()
        {
            TryInit();
            
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

        private void TryInit()
        {
            if (splitedName == null || configName != name)
            {
                OnInspectorInit();
            }
        }

        private void Init()
        {
            splitedName = name.Split('_');
            configName = name;
            var configEntityName = splitedName[0];
            EntityName = GameScopes.IsEntityName(configEntityName) ? configEntityName : null;
            int.TryParse(splitedName[^1], out currentLevel);
        }

        [OnInspectorInit]
        private void OnInspectorInit()
        {
            SetEditorColors();
            Init();

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

            cannotSetLevel = UnlockedLevels.Config.Levels.TryGetValue(EntityName, out var level) && level == currentLevel;
            canSetLevel = !cannotSetLevel;
            
            OnFirstLevel();

            for (int i = 0; i < UpgradesByScope.Count; i++)
            {
                UpgradesByScope[i].OnGUI();
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

            if (changedLevels.TryGetValue(EntityName, out var level))
            {
                if (level > currentLevel)
                {
                    changedLevels[EntityName] = currentLevel;
                    isChangedLevels = true;
                }
            }
            else
            {
                changedLevels.Add(EntityName, currentLevel);
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
                UpgradesByScope[i].level = currentLevel;
                UpgradesByScope[i].ScopeChanged -= OnScopeChanged;
                UpgradesByScope[i].ScopeChanged += OnScopeChanged;
            }

            OnScopeChanged();
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
        
        [Button("Add Scope")]
        [PropertyOrder(-1)]
        private void AddScope()
        {
            UpgradesByScope.Add(new GamePropertiesByScope());
        }
        
        
        private Color setColor = new Color(0.33f, 0.78f, 0.28f);
        private Color unsetColor = new Color(0.77f, 0.34f, 0.28f);
        private bool canSetLevel;
        private bool cannotSetLevel;
        
        private void SetEditorColors()
        {
            var tex = EditorUtils.GetTextureByColor(Color.white);
            var grayTex = EditorUtils.GetTextureByColor(new Color(0.83f, 0.83f, 0.83f));
            var gray2Tex = EditorUtils.GetTextureByColor(new Color(0.71f, 0.71f, 0.71f));
            var textColor = new Color(0.17f, 0.17f, 0.17f);
            
            var normal = SirenixGUIStyles.Button.normal;
            normal.textColor = textColor;
            normal.background = tex;
            
            var hover = SirenixGUIStyles.Button.hover;
            hover.textColor = textColor;
            hover.background = grayTex;
            
            var active = SirenixGUIStyles.Button.active;
            active.textColor = textColor;
            active.background = gray2Tex;
        }

        [Button("Set Level", ButtonSizes.Large)]
        [HorizontalGroup("LevelButtons")]
        [PropertySpace(10)]
        [GUIColor("setColor")]
        [DisableIf("cannotSetLevel")]
        private void SetLevel()
        {
            if (IsInvalid)
            {
                Debug.Log($"[{nameof(LevelConfig)}] Config is invalid! Check errors.");
            }
            else
            {
                UnlockedLevels.Config.Levels[EntityName] = currentLevel;
                OnRecomputeAll();
            }
        }
        
        [Button("Unset Level", ButtonSizes.Large)]
        [HorizontalGroup("LevelButtons")]
        [PropertySpace(10)]
        [GUIColor("unsetColor")]
        [DisableIf("canSetLevel")]
        private void UnsetLevel()
        {
            if (IsInvalid)
            {
                Debug.Log($"[{nameof(LevelConfig)}] Config is invalid! Check errors.");
            }
            else
            {
                var levels = UnlockedLevels.Config.Levels;

                if (levels.TryGetValue(EntityName, out var level))
                {
                    if (level > 1)
                    {
                        levels[EntityName] = level - 1;
                    }
                    else
                    {
                        levels.Remove(EntityName);
                    }
                    
                    OnRecomputeAll();
                }
            }
        }

        private static void OnRecomputeAll()
        {
            LevelsConfigsManager.Editor_RecomputeAllLevels();
                
            UnlockedLevels.Save();
            EntitiesProperties.Save();
            ChangedLevels.Save();
                
            UnlockedLevels.LoadOnNextAccess();
            EntitiesProperties.LoadOnNextAccess();
            ChangedLevels.LoadOnNextAccess();
                
            AssetDatabase.Refresh();
        }
    }
}
#endif
