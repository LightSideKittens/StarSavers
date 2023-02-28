using System;
using System.Collections.Generic;
using Battle.Data.GameProperty;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Battle.Data
{
    public partial class LevelsConfigsManager : SerializedScriptableObject
    {
        [Serializable]
        private class LevelsContainer
        {
            [InfoBox("Entity does not exist", InfoMessageType.Error, "$" + nameof(isEntityNotExist))]
            [InfoBox("$" + nameof(MissedLevelMessage), InfoMessageType.Error, "$" + nameof(isMissedLevel))]
            [InfoBox("$" + nameof(LevelErrorMessage), InfoMessageType.Error, "$" + nameof(isLevelError))]
            public string entityName;
            public List<LevelConfig> levels = new();
            
            [HideInInspector] public bool isEntityNotExist;
            [HideInInspector] public bool isMissedLevel;
            [HideInInspector] public int missedLevel;
            [HideInInspector] public bool isLevelError;
            [HideInInspector] public string levelErrorName;
            public string MissedLevelMessage => $"Missed level: {missedLevel}";
            public string LevelErrorMessage => $"Level Config <{levelErrorName}> has error";
        }

        public static event Action LevelUpgraded;
        private static LevelsConfigsManager Instance { get; set; }

        [InfoBox("Some configs contains errors", InfoMessageType.Error, "$" + nameof(hasError))]
        [ReadOnly] public bool hasError = true;

        [TableList, OdinSerialize, ReadOnly] private List<LevelsContainer> levelsContainers = new();
        private readonly Dictionary<string, List<LevelConfig>> levelsByEntityName = new();

        public void Init()
        {
            Instance = this;

            for (int i = 0; i < levelsContainers.Count; i++)
            {
                var levelContainer = levelsContainers[i];
                levelsByEntityName.Add(levelContainer.entityName, levelContainer.levels);
            }

            var changedLevels = ChangedLevels.Config.Levels;
            var unlockedLevels = UnlockedLevels.Config.Levels;
            
            foreach (var level in changedLevels)
            {
                if (unlockedLevels.TryGetValue(level.Key, out var levelNum))
                {
                    if (levelNum >= level.Value)
                    {
                        RecomputeAllLevels();
                        break;
                    }
                }
            }
        }

        public static void UpgradeLevel(string entityName, int level)
        {
            Debug.Log($"[{nameof(LevelsConfigsManager)}] UpgradeLevel. Entity: {entityName} | Level: {level}");
            
            if (GameScopes.IsEntityName(entityName))
            {
                var unlockedLevels = UnlockedLevels.Config.Levels;
                unlockedLevels.TryGetValue(entityName, out var currentLevel);

                if (level - currentLevel == 1)
                {
                    var dict = new Dictionary<string, List<BaseGameProperty>>();
                    Instance.levelsByEntityName[entityName][level-1].InitProperties(dict);
                    
                    ComputeLevel(dict);

                    UnlockedLevels.Config.Levels[entityName] = level;
                    LevelUpgraded?.Invoke();
                }
                else
                {
                    var diff = level - currentLevel;
                    var message = "Cannot upgrade level by more than one at once.";
                    
                    if (diff == 0)
                    {
                        message = "Cannot upgrade the same level";
                    }
                    else if(diff < 0)
                    {
                        message = "Cannot downgrade level";
                    }

                    Debug.LogError($"[{nameof(LevelsConfigsManager)}] {message} Target level: {level}, Current level: {currentLevel}");
                } 
            }
            else
            {
                Debug.LogError($"[{nameof(LevelsConfigsManager)}] Scope: {entityName} is not Entity Scope");
            }
        }

        private static void ComputeLevel(Dictionary<string, List<BaseGameProperty>> dict)
        {
            var entitiesProperties = EntitiesProperties.Config.Properties;

            foreach (var propertiesByType in dict)
            {
                var props = propertiesByType.Value;

                for (int i = 0; i < props.Count; i++)
                {
                    var prop = props[i];
                    var entitesNames = GameScopes.GetEnitiesNamesByScope(prop.scope);

                    foreach (var entityName in entitesNames)
                    {
                        if (!entitiesProperties.TryGetValue(entityName, out var propByType))
                        {
                            propByType = new Dictionary<string, ValuePercent>();
                            entitiesProperties.Add(entityName, propByType);
                        }

                        var type = propertiesByType.Key;

                        if (propByType.TryGetValue(type, out var valuePercent))
                        {
                            valuePercent.value += prop.Fixed;
                            valuePercent.percent += prop.Percent;
                            propByType[type] = valuePercent;
                        }
                        else
                        {
                            propByType.Add(type, new ValuePercent
                            {
                                value = prop.Fixed,
                                percent = prop.Percent,
                            });
                        }
                    }
                }
            }
        }

        private void RecomputeAllLevels()
        {
            Debug.Log($"[{nameof(LevelsConfigsManager)}] RecomputeAllLevels");
            ChangedLevels.Config.Levels.Clear();
            EntitiesProperties.Config.Properties.Clear();

            for (int i = 0; i < levelsContainers.Count; i++)
            {
                var levelsContainer = levelsContainers[i];

                if (UnlockedLevels.Config.Levels.TryGetValue(levelsContainer.entityName, out var unlockedLevel))
                {
                    var dict = new Dictionary<string, List<BaseGameProperty>>();
                    for (int j = 0; j < unlockedLevel; j++)
                    {
                        levelsContainer.levels[j].InitProperties(dict);
                        ComputeLevel(dict);
                    }
                }
            }
        }
    }
}