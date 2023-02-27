using System.Collections.Generic;
using Newtonsoft.Json;

namespace Core.ConfigModule
{
    internal class ConfigVersions : JsonBaseConfigData<ConfigVersions>
    {
        [JsonIgnore] public static bool IsVersionsFetched { get; set; }
        public Dictionary<string, int> versions = new();
    }
}