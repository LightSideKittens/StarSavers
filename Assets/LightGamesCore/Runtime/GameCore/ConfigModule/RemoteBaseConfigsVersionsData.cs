using System;
using System.Collections.Generic;
using Core.ConfigModule.Attributes;
using Newtonsoft.Json;
using UnityEngine;

namespace Core.ConfigModule
{
    [Generatable]
    public class RemoteBaseConfigsVersionsData : JsonBaseConfigData<RemoteBaseConfigsVersionsData>
    {
        public static RemoteBaseConfigsVersionsData remoteBaseInstance;
        
        [JsonProperty]
        private Dictionary<string, int> configsVersions = new Dictionary<string, int>
        {
            {"gameEventConditionsData", 0}
        };

        public static bool CompareRemoteToLocal(string key)
        {
            if (remoteBaseInstance != null)
            {
                if (remoteBaseInstance.configsVersions.TryGetValue(key, out var remoteVersion))
                {
                    if (Config.configsVersions.TryGetValue(key, out var localVersion))
                    {
                        return localVersion == remoteVersion;
                    }
                }
            }

            Debug.LogWarning($"{nameof(remoteBaseInstance)} is null. Check: 1. Your internet connection. 2. That the file is on the server.");

            return false;
        }
        public static void Sync(string key) => Config.configsVersions[key] = remoteBaseInstance.configsVersions[key];

        protected override void DefaultFetch(Action callback)
        {
            SendRequest(instance.FileName, callback, () =>
            {
                remoteBaseInstance = instance;
                Load();
            });
        }
    }
}