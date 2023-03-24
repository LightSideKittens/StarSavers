using System.Collections.Generic;
using Core.ConfigModule;
using Newtonsoft.Json;

namespace Battle.Data
{
    public class UnlockedLevels : JsonBaseConfigData<UnlockedLevels>
    {
        [JsonProperty("levels")] private Dictionary<string, int> byName = new();
        public static Dictionary<string, int> ByName => Config.byName;
    }
}