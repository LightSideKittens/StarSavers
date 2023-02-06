#if !UNITY_EDITOR
#define NOT_EDITOR
#endif

using System;
using System.Diagnostics;
using System.IO;
using Core.ConfigModule.Server.BaseRemoteConfig;
using Newtonsoft.Json;
using UnityEngine;
using static Core.ConfigModule.FolderNames;
using Debug = UnityEngine.Debug;

namespace Core.ConfigModule
{
    [Serializable]
    public abstract class BaseConfig<T> where T : BaseConfig<T>, new()
    {
        protected static JsonSerializerSettings Settings { get; } = new JsonSerializerSettings
        {
            ContractResolver = UnityJsonContractResolver.Instance
        };

        public static bool IsNull => instance == null;
        public static bool IsLoaded { get; private set; }
        private static bool needLoadFromInstance;
        
        protected static T instance;
        
        private static Func<T> getter;

        static BaseConfig()
        {
            getter = StaticConstructor;
            BaseRemoteConfig<T>.Fetching += OnFetching;
        }
        
        protected static void LoadOnNextAccess(bool fromInstance = true)
        {
            needLoadFromInstance = fromInstance;
            getter = ResetGetter;
        }

        private static T ResetGetter()
        {
            instance = new T();
            Load();

            return instance;
        }

        private static T StaticConstructor()
        {
            CreateInstance();
            Load();

            return instance;
        }

        protected abstract string FullFileName { get; }

        protected abstract string FolderPath { get; }

        protected abstract string Ext { get; }
        public static T Config => getter();

        protected abstract string FileName { get; set; }
        protected virtual string FolderName => GetType().Name;
        
        protected virtual void SetDefault(){}
        protected virtual void OnLoading(){}
        protected virtual void OnLoaded(){}
        protected virtual void OnSaving(){}
        protected virtual void OnSaved(){}

        public static void Initialize() => getter();

        [Conditional("UNITY_EDITOR")]
        private static void Generate()
        {
            instance = new T();
            Load();
            Save();
        }

        protected static T Load()
        {
            instance.OnLoading();
            
            var fullFileName = instance.FullFileName;
            string json = string.Empty;

            if (needLoadFromInstance)
            {
                needLoadFromInstance = false;
                
                if (File.Exists(fullFileName))
                {
                    json = File.ReadAllText(fullFileName);
                }
                else
                {
                    json = Resources.Load<TextAsset>(Path.Combine(instance.FolderName, instance.FileName))?.text;
                }
            }

            if (string.IsNullOrEmpty(json) == false)
            {
                Deserialize(json);
            }
            else
            {
                instance.SetDefault();
                getter = GetInstance;
            }
            
            IsLoaded = true;
            instance.OnLoaded();
            
            return instance;
        }

        public static void Save() => Set(instance);

#if UNITY_EDITOR
        public static void SaveAsDefault()
        {
            var folderPath = Path.Combine(Application.dataPath, Configs, DefaultSaveData, instance.FolderName);
            var fullFileName = Path.Combine(folderPath, $"{instance.FileName}.{instance.Ext}");
            Internal_Set(instance, folderPath, fullFileName);
        }
#endif

        public static void Set(T config)
        {
            Internal_Set(config, config.FolderPath, config.FullFileName);
        }

        private static void Internal_Set(T config, string folderPath, string fullFileName)
        {
            instance.OnSaving();
            
            if (Directory.Exists(folderPath) == false)
            {
                Directory.CreateDirectory(folderPath);
            }

            var json = Serialize(config);
            
            CheckFileNameIsNullOrEmpty();
            File.WriteAllText(fullFileName,json);
            ConfigsTypeData.AddPath(fullFileName);
            instance.OnSaved();
        }
        
        [Conditional("UNITY_EDITOR")]
        private static void CheckFileNameIsNullOrEmpty()
        {
            if (string.IsNullOrEmpty(instance.FileName))
            {
                instance.FileName = "NULL_NAME";
            }
        }

        private static T GetInstance() => instance;
        
        private static void CreateInstance()
        {
            UnityEventWrapper.UnSubscribeFromApplicationPausedEvent(Save);
            UnityEventWrapper.SubscribeToApplicationPausedEvent(Save);
            instance = new T();
        }

        public static void UpdateName(string newName)
        {
            if (instance.FileName.Equals(newName) == false)
            {
                getter = Load;
                instance.FileName = newName;
            }
        }

        public static void Fetch(Action callback) => BaseRemoteConfig<T>.Fetch(callback);
        protected virtual void DefaultFetch(Action callback) => BaseRemoteConfig<T>.DefaultFetch(callback);
        protected static void SendRequest(string key, Action callback, Action onSuccess = null) => BaseRemoteConfig<T>.SendRequest(key, callback, onSuccess);

        private static void OnFetching()
        {
            var remoteConfig = BaseRemoteConfig<T>.instance;
            remoteConfig.createInstance = CreateInstance;
            remoteConfig.load = Load;
            remoteConfig.onLoading = () => instance.OnLoading();
            remoteConfig.onLoaded = () => instance.OnLoading();
            remoteConfig.deserialize = Deserialize;
            remoteConfig.fileNameAction = () => instance.FileName;
            remoteConfig.defaultFetch = callback => instance.DefaultFetch(callback);
            BaseRemoteConfig<T>.Fetching -= OnFetching;
        }
        
        private static void Deserialize(string json)
        {
            instance = JsonConvert.DeserializeObject<T>(json, Settings);
            getter = GetInstance;
        }

        private static string Serialize(T config)
        {
            var json = string.Empty;
            Serialize_Editor(config, ref json);
            Serialize_Runtime(config, ref json);

            return json;
        }

        [Conditional("UNITY_EDITOR")]
        private static void Serialize_Editor(T config, ref string json)
        {
            json = JsonConvert.SerializeObject(config, Formatting.Indented, Settings);
        }
        
        [Conditional("NOT_EDITOR")]
        private static void Serialize_Runtime(T config, ref string json)
        {
            json = JsonConvert.SerializeObject(config, Settings);
        }
    }
}