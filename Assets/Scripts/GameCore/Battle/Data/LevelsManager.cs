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
            public int entityId;
            public List<LevelConfig> levels = new();
        }
        
        public static event Action LevelUpgraded;
        private static LevelsManager Instance { get; set; }

        [InfoBox("Some configs contains errors", InfoMessageType.Error, "$" + nameof(hasError))]
        [ReadOnly] public bool hasError = true;

        [TableList, OdinSerialize, ReadOnly] private List<LevelsContainer> levelsContainers = new();
        private readonly Dictionary<int, List<LevelConfig>> levelsByEntityId = new();

        public void Init()
        {
            Burger.Log($"[{nameof(LevelsManager)}] Init");
            Instance = this;
            levelsByEntityId.Clear();
            
            for (int i = 0; i < levelsContainers.Count; i++)
            {
                var levelContainer = levelsContainers[i];
                levelsByEntityId.Add(levelContainer.entityId, levelContainer.levels);
            }

            RecomputeAllLevels();
        }

        public static bool CanUpgrade(int entityId)
        {
            if (EntityMeta.IsEntityId(entityId))
            {
                UnlockedLevels.EntitiesLevel.TryGetValue(entityId, out var currentLevel);
                
                var levels = Instance.levelsByEntityId[entityId];
                return currentLevel < levels.Count;
            }
            
            return false;
        }

        public static void UpgradeLevel(int entityId)
        {
            if (CanUpgrade(entityId))
            {
                var entitiesLevel = UnlockedLevels.EntitiesLevel;
                entitiesLevel.TryGetValue(entityId, out var currentLevel);
                var level = Instance.levelsByEntityId[entityId][currentLevel];
                
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

            var entityIds = UnlockedLevels.EntityIdByUpgradesOrder;
            
            for (int i = 0; i < entityIds.Count; i++)
            {
                var data = entityIds[i];
                var level = levelsByEntityId[data.entityId][data.level];
                ApplyLevel(level);
            }
        }
    }
}