using System;
using System.Collections.Generic;
using Core.Server;
using Firebase.Extensions;
using Firebase.Firestore;
using Newtonsoft.Json;
using static Core.Server.User;

namespace Core.ConfigModule
{
    public abstract class DatabaseRemoteConfig<T, TConfig> where T : DatabaseRemoteConfig<T, TConfig>, new() where TConfig : BaseConfig<TConfig>, new()
    {
        private static Func<DocumentReference> getter;
        private static readonly T instance;
        protected static string UserId { get; private set; }

        static DatabaseRemoteConfig()
        {
            instance = new T();
            getter = GetReference;
        }

        private static DocumentReference GetReference()
        {
            getter = GetCreatedReference;

            return instance.Reference;
        }
        
        protected abstract DocumentReference Reference { get; }
        private static DocumentReference GetCreatedReference() => instance.Reference;

        public static void Push(Action onSuccess = null, Action onError = null)
        {
            if (!ServerEnabled)
            { 
                onSuccess?.Invoke();
                return; 
            }
            
            Internal_Push(onSuccess, onError);
        }

        private static void Internal_Push(Action onSuccess, Action onError)
        {
            SignIn(() =>
            {
                Burger.Log($"[{typeof(TConfig).Name}] Push");
                var docRef = getter();
                var config = BaseConfig<TConfig>.Config;
                var json = JsonConvert.SerializeObject(config, config.Settings);
                var dict = new Dictionary<string, object>()
                {
                    {BaseConfig<TConfig>.Config.FileName, json}
                };
                var pushTask = docRef.SetAsync(dict);
                
                pushTask.ContinueWithOnMainThread(task =>
                {
                    if (task.IsCompletedSuccessfully)
                    {
                        Burger.Log($"[{typeof(TConfig).Name}] Success Push");
                        onSuccess.SafeInvoke();
                    }
                    else
                    {
                        Burger.Error($"[{typeof(TConfig).Name}] Failure Push: {task.Exception.Message}");
                        onError.SafeInvoke();
                    }
                });
            }, onError);
        }
        
        public static void Fetch(string userId, Action<TConfig> onSuccess, Action onError = null)
        {
            if (!ServerEnabled)
            { 
                onSuccess.SafeInvoke(BaseConfig<TConfig>.Config);
                return; 
            }
            
            Internal_Fetch(userId, onSuccess: dict =>
            {
                var config = BaseConfig<TConfig>.Config;
                var json = (string) dict[config.FileName];
                config = JsonConvert.DeserializeObject<TConfig>(json, config.Settings);
                onSuccess.SafeInvoke(config);
            }, onError, onError);
        }

        public static void Fetch(Action onSuccess = null, Action onError = null)
        {
            if (!ServerEnabled)
            { 
                onSuccess.SafeInvoke();
                return; 
            }
            
            OnAppPause.UnSubscribe(OnApplicationPause);
            OnAppPause.Subscribe(OnApplicationPause);
            Internal_Fetch(Id, onSuccess: dict =>
            {
                var config = BaseConfig<TConfig>.Config;
                var json = (string) dict[config.FileName];
                BaseConfig<TConfig>.Deserialize(json);
                onSuccess.SafeInvoke();
            }, onError, () =>
            {
                Storage<TConfig>.Fetch(OnComplete, OnComplete);

                void OnComplete()
                {
                    Push(onSuccess, onError);
                }
            });
        }

        private static void Internal_Fetch(string userId, Action<Dictionary<string, object>> onSuccess = null,
            Action onError = null,
            Action onResponseEmpty = null)
        {
            SignIn(() =>
            {
                Burger.Log($"[{typeof(TConfig).Name}] Fetch");

                if (string.IsNullOrEmpty(userId))
                {
                    Burger.Warning($"[{typeof(TConfig).Name}] UserId is Null or Empty!");
                    userId = Id;
                }
                
                UserId = userId;
                var docRef = getter();

                docRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
                {
                    if (task.IsCompletedSuccessfully)
                    {
                        var dict = task.Result.ToDictionary();

                        if (dict == null)
                        {
                            if (onResponseEmpty != null)
                            {
                                Burger.Log($"[{typeof(TConfig).Name}] Failure Fetch: Response is empty. User: {userId}");
                                onResponseEmpty.SafeInvoke();
                            }
                            else
                            {
                                Burger.Error($"[{typeof(TConfig).Name}] Failure Fetch: Response is empty. User: {userId}");
                            }
                            
                            onError.SafeInvoke();
                            return;
                        }
                        
                        Burger.Log($"[{typeof(TConfig).Name}] Success Fetch. User: {userId}"); 
                        onSuccess.SafeInvoke(dict);
                    }
                    else
                    {
                        Burger.Error($"[{typeof(TConfig).Name}] Failure Fetch {task.Exception.Message}. User: {userId}");
                        onError.SafeInvoke();
                    }
                });
            }, onError);
        }

        private static void OnApplicationPause()
        {
            Push();
        }
    }
}