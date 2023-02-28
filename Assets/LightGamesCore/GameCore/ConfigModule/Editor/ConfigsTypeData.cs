using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

namespace Core.ConfigModule
{
    public partial class ConfigsTypeData : JsonBaseConfigData<ConfigsTypeData>
    {
        protected override string DefaultFolderName => Path.Combine("EditorConfigs", GetType().Name);
        [JsonProperty] private readonly HashSet<string> paths = new HashSet<string>();
        private static TextAsset selectedTextAsset;
        private static readonly Dictionary<string, Action> loadOnNextAccessActions = new Dictionary<string, Action>();

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

        [Conditional("UNITY_EDITOR")]
        public static void AddLoadOnNextAccessAction(string name, Action action)
        {
            if (Application.isEditor)
            {
                loadOnNextAccessActions.TryAdd(name, action);
            }
        }

        [Conditional("UNITY_EDITOR")]
        public static void CallLoadOnNextAccess(string name)
        {
            if (Application.isEditor)
            {
                loadOnNextAccessActions.TryGetValue(name, out var action);
                action?.Invoke();
            }
        }
    }
}