using System.Collections.Generic;
using LGCore.ConfigModule;
using Newtonsoft.Json;

namespace Battle.Data
{
    public class EntiProps : BaseConfig<EntiProps>
    {
        [JsonProperty("props")] 
        public Dictionary<string, Dictionary<string, ValuePercent>> byName = new();
        public static Dictionary<string, Dictionary<string, ValuePercent>> ByName => Config.byName;
    }
}