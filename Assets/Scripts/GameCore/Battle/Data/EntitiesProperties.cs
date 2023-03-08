using System.Collections.Generic;
using Core.ConfigModule;
using Newtonsoft.Json;

namespace Battle.Data
{
    public class EntitiesProperties : JsonBaseConfigData<EntitiesProperties>
    {
        [JsonProperty("Properties")] 
        private Dictionary<string, Dictionary<string, ValuePercent>> byName = new();

        [JsonIgnore]
        public static Dictionary<string, Dictionary<string, ValuePercent>> ByName => Config.byName;
    }
}