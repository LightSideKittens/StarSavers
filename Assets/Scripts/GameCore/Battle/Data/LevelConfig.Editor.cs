#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Battle.Data.GameProperty;
using Core.ConfigModule;
using Core.Server;
using Firebase.Extensions;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEditor;
using UnityEngine;
using static Battle.Data.LevelsConfigsManager;

namespace Battle.Data
{
    public partial class LevelConfig
    {
        private int currentLevel;
        private bool isFirstLevelError;
        private bool hasScopeError;
        private bool hasEmptyScopeError;
        private bool isSubscribed;
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
            environment ??= Local;
            OnEnvironmentChanged();
        }

        [OnInspectorInit]
        private void OnInspectorInit()
        {
            Init();

            if (IsInvalidName)
            {
                return;
            }

            OnUpgradeStepsChanged();
        }


        private static bool isButtonStyleSetted;
        
        [OnInspectorGUI]
        private void OnInspectorGUI()
        {
            if (!isButtonStyleSetted)
            {
                EditorUtils.SetSirenixButtonWhiteColor();
                isButtonStyleSetted = true;
            }
            
            if (IsInvalidName)
            {
                return;
            }

            cannotSetLevel = UnlockedLevels.ByName.TryGetValue(EntityName, out var level) && level == currentLevel;
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
            for (int i = 0; i < UpgradesByScope.Count; i++)
            {
                UpgradesByScope[i].ScopeChanged -= OnScopeChanged;
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

        private Color setColor = new Color(0.35f, 0.85f, 0.29f);
        private Color unsetColor = new Color(0.84f, 0.35f, 0.29f);
        private bool canSetLevel;
        private bool cannotSetLevel;
        private const string Local = nameof(Local);
        private const string DefaultServer = "Default & Server";
        private IEnumerable<string> configsEnvironment => new[] {Local, DefaultServer};
        private bool CanUseDefaultServer => environment == DefaultServer && currentLevel == 1;
        private bool IsConfirmedPush => EditorUtility.DisplayDialog(
            $"Pushing", 
            $"Push config: {name}?", 
            "Yes", 
            "No");
        
        [OdinSerialize] 
        [ValueDropdown("configsEnvironment")]
        [PropertyOrder(1)]
        [OnValueChanged("OnEnvironmentChanged")]
        private string environment = Local;

        private void OnEnvironmentChanged()
        {
            if (CanUseDefaultServer)
            {
                UnlockedLevels.LoadAsDefault();
            }
            else
            {
                UnlockedLevels.LoadOnNextAccess();
            }
        }

        [Button("Set Level", ButtonSizes.Large)]
        [HorizontalGroup("LevelButtons")]
        [PropertySpace(10)]
        [GUIColor("setColor")]
        [DisableIf("cannotSetLevel")]
        [PropertyOrder(2)]
        private void SetLevel()
        {
            if (IsInvalid)
            {
                Burger.Error($"[{nameof(LevelConfig)}] Config is invalid! Check errors.");
            }
            else
            {
                if (CanUseDefaultServer)
                {
                    if (IsConfirmedPush)
                    {
                        UnlockedLevels.LoadAsDefault();
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    UnlockedLevels.LoadOnNextAccess();
                }
                
                UnlockedLevels.ByName[EntityName] = currentLevel;
                RecomputeAllAndSave();
            }
        }
        
        [Button("Unset Level", ButtonSizes.Large)]
        [HorizontalGroup("LevelButtons")]
        [PropertySpace(10)]
        [GUIColor("unsetColor")]
        [DisableIf("canSetLevel")]
        [PropertyOrder(2)]
        private void UnsetLevel()
        {
            if (IsInvalid)
            {
                Burger.Log($"[{nameof(LevelConfig)}] Config is invalid! Check errors.");
            }
            else
            {
                if (CanUseDefaultServer)
                {
                    if (IsConfirmedPush)
                    {
                        UnlockedLevels.LoadAsDefault();
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    UnlockedLevels.LoadOnNextAccess();
                }
                
                var levels = UnlockedLevels.ByName;

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
                    
                    RecomputeAllAndSave();
                }
            }
        }

        private void RecomputeAllAndSave()
        {
            Editor_RecomputeAllLevels();

            if (CanUseDefaultServer)
            {
                UnlockedLevels.Editor_SaveAsDefault();
                EntiProps.Editor_SaveAsDefault();
            }

            UnlockedLevels.Save();
            EntiProps.Save();

            if (!CanUseDefaultServer)
            {
                UnlockedLevels.LoadOnNextAccess();
                EntiProps.LoadOnNextAccess();
            }
            else
            {
                EditorUtility.DisplayProgressBar("Pushing...", "Pushing in progress...", 0.5f);
                Admin.SignIn(() =>
                {
                    var configsRef = Admin.Storage.RootReference.Child(FolderNames.Configs);
                    var unlockedLevelsRef = configsRef.Child($"{UnlockedLevels.Config.FileName}.json");
                    var entiPropsRef = configsRef.Child($"{EntiProps.Config.FileName}.json");
                    unlockedLevelsRef.PutFileAsync(UnlockedLevels.Config.FullFileName).ContinueWithOnMainThread(task => OnComplete(task, "unlockedLevels"));
                    entiPropsRef.PutFileAsync(EntiProps.Config.FullFileName).ContinueWithOnMainThread(task => OnComplete(task, "entiProps"));

                    void OnComplete(Task task, string name)
                    {
                        EditorUtility.ClearProgressBar();
                        if (task.IsCompletedSuccessfully)
                        {
                            Burger.Log($"[{nameof(LevelConfig)}] Push Success! Config: {name}");
                        }
                        else
                        {
                            Burger.Error($"[{nameof(LevelConfig)}] Push Failure! Config: {name}. Error: {task.Exception.Message}");
                        }
                    }
                });
            }

            AssetDatabase.Refresh();
        }
    }
}
#endif
