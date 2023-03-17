using System.Collections.Generic;
using System.Diagnostics;
using Newtonsoft.Json;

namespace Core.ConfigModule
{
    public partial class ConfigsTypeData : JsonBaseConfigData<ConfigsTypeData>
    {
        protected override string FolderName => FolderNames.EditorConfigs;
        [JsonProperty] private readonly HashSet<string> paths = new();

        public static void Init()
        {
            Initialize();
        }

        public new static void Save()
        {
            Set(instance);
        }

        [Conditional("UNITY_EDITOR")]
        public static void AddPath(string path)
        {
            Config.paths.Add(path);
        }
    }
}