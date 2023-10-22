using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace Battle.Data
{
    public partial class LevelsManager : SerializedScriptableObject
    {
        [Serializable]
        private class LevelsContainer
        {
            [EntityId]
            [ReadOnly] public int entityId;
            [ValueDropdown("Levels", IsUniqueList = true)]
            public List<LevelConfig> levels = new();
            
#if UNITY_EDITOR
            private IEnumerable<LevelConfig> Levels => AssetDatabaseUtils.LoadAllAssets<LevelConfig>(EntityMeta.EntityIds.GetNameById(entityId));

            public override bool Equals(object obj)
            {
                if (obj is LevelsContainer drawer)
                {
                    return Equals(drawer);
                }

                return false;
            }
        
            public bool Equals(LevelsContainer other)
            {
                return entityId == other.entityId;
            }

            public override int GetHashCode()
            {
                return entityId.GetHashCode();
            }
#endif
        }
        
        public static event Action LevelUpgraded;
        private static LevelsManager Instance { get; set; }

        [TableList, OdinSerialize, ValueDropdown("AvailableContainer", IsUniqueList = true)]
        [HideReferenceObjectPicker]
        private HashSet<LevelsContainer> levelsContainers = new();
        
        private readonly Dictionary<int, List<LevelConfig>> levelsByEntityId = new();

        public void Init()
        {
            Burger.Log($"[{nameof(LevelsManager)}] Init");
            Instance = this;
            levelsByEntityId.Clear();

            foreach (var levelContainer in levelsContainers)
            {
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
                
                level.Apply();
                UnlockedLevels.UpgradeLevel(level);
                
                LevelUpgraded?.Invoke();
            }
        }

        private void RecomputeAllLevels()
        {
            Burger.Log($"[{nameof(LevelsManager)}] RecomputeAllLevels");
            EntiProps.Clear();

            var entityIds = UnlockedLevels.EntityIdByUpgradesOrder;
            
            for (int i = 0; i < entityIds.Count; i++)
            {
                var data = entityIds[i];
                var level = levelsByEntityId[data.entityId][data.level-1];
                level.Apply();
            }
        }
    }
}