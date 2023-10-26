using System;
using System.Collections.Generic;
using LSCore.ConfigModule;
using Newtonsoft.Json;

namespace Battle.Data
{
    public class UnlockedLevels : BaseConfig<UnlockedLevels>
    {
        [Serializable]
        public struct UpgradeData
        {
            public int entityId;
            public int level;
        }
        
        [JsonProperty("upgrades")] private List<UpgradeData> entityIdByUpgradesOrder = new();
        [JsonProperty("entitiesLevel")] private Dictionary<int, int> entitiesLevel = new();
        public static List<UpgradeData> EntityIdByUpgradesOrder => Config.entityIdByUpgradesOrder;
        public static Dictionary<int, int> EntitiesLevel => Config.entitiesLevel;
        
        public static void UpgradeLevel(LevelConfig levelConfig)
        {
            var data = (levelConfig.EntityId, levelConfig.Level);
            EntityIdByUpgradesOrder.Add(new UpgradeData(){entityId = data.EntityId, level = data.Level});
            EntitiesLevel[data.EntityId] = data.Level;
        }
    }
}