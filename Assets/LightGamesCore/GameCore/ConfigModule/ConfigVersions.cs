using System.Collections.Generic;
using Newtonsoft.Json;

namespace Core.ConfigModule
{
    public class ConfigVersions : JsonBaseConfigData<ConfigVersions>
    {
        [JsonProperty] private Dictionary<string, int> localVersions = new();
        private static Dictionary<string, int> remoteVersions = new();

        public static int RemoteCount => remoteVersions.Count;

        public static void Set<T>(int version) where T : BaseConfig<T>, new()
        {
            Config.localVersions[BaseConfig<T>.Config.FileName] = version;
        }
        
        public static int GetLocal<T>() where T : BaseConfig<T>, new()
        {
            return GetVersion<T>(Config.localVersions);
        }
        
        public static int GetRemote<T>() where T : BaseConfig<T>, new()
        {
            return GetVersion<T>(remoteVersions);
        }
        
        private static int GetVersion<T>(Dictionary<string, int> versions) where T : BaseConfig<T>, new()
        {
            var name = BaseConfig<T>.Config.FileName;
            versions.TryGetValue(name, out var version);

            return version;
        }

        public static bool Compare<T>() where T : BaseConfig<T>, new()
        {
            return Compare(BaseConfig<T>.Config.FileName);
        }
        
        public static bool Compare(string name)
        {
            if (remoteVersions.TryGetValue(name, out var remote) && Config.localVersions.TryGetValue(name, out var local))
            {
                return local == remote;
            }

            return false;
        }

        public static void Update<T>() where T : BaseConfig<T>, new()
        {
            Update(BaseConfig<T>.Config.FileName);
        }
        
        public static void Update(string name)
        {
            if (remoteVersions.TryGetValue(name, out var remote) && Config.localVersions.TryGetValue(name, out _))
            {
                Config.localVersions[name] = remote;
            }
        }
        
        public static void Increase<T>() where T : BaseConfig<T>, new()
        {
            Increase(BaseConfig<T>.Config.FileName);
        }
        
        public static void Increase(string name)
        {
            Config.localVersions.TryGetValue(name, out var version);
            Config.localVersions[name] = version + 1;
        }
        
        public static void SetRemote(string json)
        {
            var config = JsonConvert.DeserializeObject<ConfigVersions>(json, Config.Settings);
            remoteVersions = config.localVersions;
        }
    }
}