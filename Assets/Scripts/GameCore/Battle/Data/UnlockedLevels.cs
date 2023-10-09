using System.Collections.Generic;
using LSCore.ConfigModule;
using Newtonsoft.Json;

namespace Battle.Data
{
    public class UnlockedLevels : BaseConfig<UnlockedLevels>
    {
        [JsonProperty("upgrades")] private List<(int entityName, int level)> entityIdByUpgradesOrder = new();
        [JsonProperty("entitiesLevel")] private Dictionary<int, int> entitiesLevel = new();
        public static List<(int entityName, int level)> EntityIdByUpgradesOrder => Config.entityIdByUpgradesOrder;
        public static Dictionary<int, int> EntitiesLevel => Config.entitiesLevel;
        
        public static void UpgradeLevel(LevelConfig levelConfig)
        {
            var data = (levelConfig.entityName, levelConfig.Level);
            EntityIdByUpgradesOrder.Add(data);
            EntitiesLevel[data.entityName] = data.Level;
        }
    }
}