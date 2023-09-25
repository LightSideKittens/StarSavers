using System.Collections.Generic;
using LGCore.ConfigModule;
using Newtonsoft.Json;

namespace Battle.Data
{
    public class UnlockedLevels : BaseConfig<UnlockedLevels>
    {
        [JsonProperty("levels")] private Dictionary<string, int> byName = new();
        public static Dictionary<string, int> ByName => Config.byName;
    }
}