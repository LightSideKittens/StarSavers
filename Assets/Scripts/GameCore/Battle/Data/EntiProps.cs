using System.Collections.Generic;
using Core.ConfigModule;
using Newtonsoft.Json;

namespace Battle.Data
{
    public class EntiProps : JsonBaseConfigData<EntiProps>
    {
        [JsonProperty("props")] 
        public Dictionary<string, Dictionary<string, ValuePercent>> byName = new();
        public static Dictionary<string, Dictionary<string, ValuePercent>> ByName => Config.byName;
    }
}