using System.Collections.Generic;
using Newtonsoft.Json;

namespace Core.ConfigModule
{
    internal class ConfigVersions : JsonBaseConfigData<ConfigVersions>
    {
        public Dictionary<string, int> versions = new();
    }
}