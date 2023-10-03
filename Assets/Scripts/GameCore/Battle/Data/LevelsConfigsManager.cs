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
            Burger.Log($"[{nameof(LevelsConfigsManager)}] Init");
            Instance = this;
            levelsByEntityName.Clear();
            
            for (int i = 0; i < levelsContainers.Count; i++)
            {
                var levelContainer = levelsContainers[i];
                levelsByEntityName.Add(levelContainer.entityName, levelContainer.levels);
            }

            RecomputeAllLevels();
        }

        public static bool CanUpgrade(string entityName)
        {
            if (GameScopes.IsEntityName(entityName))
            {
                UnlockedLevels.ByName.TryGetValue(entityName, out var currentLevel);
                
                var levels = Instance.levelsByEntityName[entityName];
                return currentLevel < levels.Count;
            }
            
            return false;
        }

        public static void UpgradeLevel(string entityName)
        {
            if (CanUpgrade(entityName))
            {
                var unlockedLevels = UnlockedLevels.ByName;
                unlockedLevels.TryGetValue(entityName, out var currentLevel);

                var dict = new Dictionary<string, List<BaseGameProperty>>();
                Instance.levelsByEntityName[entityName][currentLevel].InitProperties(dict);
                
                ApplyLevel(dict);

                UnlockedLevels.ByName[entityName] = currentLevel + 1;
                LevelUpgraded?.Invoke();
            }
        }

        private static void ApplyLevel(Dictionary<string, List<BaseGameProperty>> dict)
        {
            var entitiesProperties = EntiProps.ByName;

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
                            valuePercent.value += prop;
                            valuePercent.percent += prop.percent;
                            propByType[type] = valuePercent;
                        }
                        else
                        {
                            propByType.Add(type, new ValuePercent
                            {
                                value = prop.value,
                                percent = prop.percent,
                            });
                        }
                    }
                }
            }
        }

        private void RecomputeAllLevels()
        {
            Burger.Log($"[{nameof(LevelsConfigsManager)}] RecomputeAllLevels");
            EntiProps.ByName.Clear();

            for (int i = 0; i < levelsContainers.Count; i++)
            {
                var levelsContainer = levelsContainers[i];

                if (UnlockedLevels.ByName.TryGetValue(levelsContainer.entityName, out var unlockedLevel))
                {
                    var dict = new Dictionary<string, List<BaseGameProperty>>();
                    
                    for (int j = 0; j < unlockedLevel; j++)
                    {
                        levelsContainer.levels[j].InitProperties(dict);
                    }
                    
                    ApplyLevel(dict);
                }
            }
        }
    }
}