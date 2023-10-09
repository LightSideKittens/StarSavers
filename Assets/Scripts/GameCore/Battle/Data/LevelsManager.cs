using System;
using System.Collections.Generic;
using Battle.Data.GameProperty;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace Battle.Data
{
    public partial class LevelsManager : SerializedScriptableObject
    {
        [Serializable]
        private class LevelsContainer
        {
            public int entityName;
            public List<LevelConfig> levels = new();
        }
        
        public static event Action LevelUpgraded;
        private static LevelsManager Instance { get; set; }

        [InfoBox("Some configs contains errors", InfoMessageType.Error, "$" + nameof(hasError))]
        [ReadOnly] public bool hasError = true;

        [TableList, OdinSerialize, ReadOnly] private List<LevelsContainer> levelsContainers = new();
        private readonly Dictionary<int, List<LevelConfig>> levelsByEntityName = new();

        public void Init()
        {
            Burger.Log($"[{nameof(LevelsManager)}] Init");
            Instance = this;
            levelsByEntityName.Clear();
            
            for (int i = 0; i < levelsContainers.Count; i++)
            {
                var levelContainer = levelsContainers[i];
                levelsByEntityName.Add(levelContainer.entityName, levelContainer.levels);
            }

            RecomputeAllLevels();
        }

        public static bool CanUpgrade(int entityName)
        {
            if (EntityMeta.IsEntityName(entityName))
            {
                UnlockedLevels.EntitiesLevel.TryGetValue(entityName, out var currentLevel);
                
                var levels = Instance.levelsByEntityName[entityName];
                return currentLevel < levels.Count;
            }
            
            return false;
        }

        public static void UpgradeLevel(int entityName)
        {
            if (CanUpgrade(entityName))
            {
                var entitiesLevel = UnlockedLevels.EntitiesLevel;
                entitiesLevel.TryGetValue(entityName, out var currentLevel);
                var level = Instance.levelsByEntityName[entityName][currentLevel];
                
                ApplyLevel(level);
                UnlockedLevels.UpgradeLevel(level);
                
                LevelUpgraded?.Invoke();
            }
        }

        private static void ApplyLevel(LevelConfig level)
        {
            var entiProps = EntiProps.ByName;
            
            
        }

        private void RecomputeAllLevels()
        {
            Burger.Log($"[{nameof(LevelsManager)}] RecomputeAllLevels");
            EntiProps.Clear();

            var entityNames = UnlockedLevels.EntityIdByUpgradesOrder;
            
            for (int i = 0; i < entityNames.Count; i++)
            {
                var data = entityNames[i];
                var level = levelsByEntityName[data.entityName][data.level];
                ApplyLevel(level);
            }
        }
    }
}