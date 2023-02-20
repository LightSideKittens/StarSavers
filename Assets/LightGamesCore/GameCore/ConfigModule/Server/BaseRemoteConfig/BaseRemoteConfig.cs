using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Core.ConfigModule.Server.BaseRemoteConfig
{
    public abstract class BaseRemoteConfig<T> where T : BaseConfig<T>, new()
    {
        public static BaseRemoteConfig<T> instance;
        public static event Action Fetching;
        public Action createInstance;
        public Action<Action> defaultFetch;
        public Func<T> load;
        public Action onLoading;
        public Action onLoaded;
        public Action<string> deserialize;
        public Func<string> fileNameAction;
        
        protected Action onError;
        
        private Action<Action> fetcher;
        private Action callbacks;

        protected BaseRemoteConfig()
        {
            instance = this;
            fetcher = FirstFetch;
            onError = OnFirstError;
        }
        
        public static void Fetch(Action callback)
        {
            Fetching?.Invoke();
            instance.fetcher(callback);
        }
        
        private static async void FirstFetch(Action callback)
        {
            if (BaseConfig<T>.IsNull)
            {
                instance.createInstance();
            }
            
            await instance.Login();
            instance.defaultFetch(callback);
            instance.fetcher = instance.defaultFetch;
        }

        public static void SendRequest(string key, Action callback, Action onSuccess = null)
        {
            if (instance.AddCallback(callback))
            {
                return;
            }
            
            instance.Internal_SendRequest(key, callback, onSuccess);
        }

        public static void DefaultFetch(Action callback) => instance.Internal_DefaultFetch(callback);
        
        protected abstract UniTask Login();
        protected abstract void Internal_SendRequest(string key, Action callback, Action onSuccess = null);

        private void Internal_DefaultFetch(Action callback)
        {
            var fileName = fileNameAction();
            
            if (RemoteBaseConfigsVersionsData.CompareRemoteToLocal(fileName) == false)
            {
                SendRequest(fileName, callback, () =>
                {
                    RemoteBaseConfigsVersionsData.Sync(fileName);
                });
            }
            else
            {
                AddCallback(callback);
                onError();
            }
        }
        
        private bool AddCallback(Action callback)
        {
            var isFetchInProgress = callbacks != null;

            callbacks += callback;
            callbacks += OnCallback;

            return isFetchInProgress;
            
            void OnCallback()
            {
                callbacks -= callback;
                callbacks -= OnCallback;
            }
        }
        
        protected void OnSuccess(string json, Action onSuccess = null)
        {
            onLoading();
            deserialize(json);
            onLoaded();
            onSuccess?.Invoke();
            callbacks();
        }

        private void OnFirstError()
        {
            if (BaseConfig<T>.IsLoaded)
            {
                onError = OnError;
            }
            else
            {
                load();
            }
            
            callbacks();
        }
        
        private void OnError() => callbacks();
    }
}