using System;
using System.Collections.Generic;
using Core.Server;
using Firebase.Extensions;
using Firebase.Firestore;
using Newtonsoft.Json;
using UnityEngine;
#if DEBUG
using static Core.ConfigModule.BaseConfig<Core.ConfigModule.DebugData>;
#endif

namespace Core.ConfigModule
{
    public abstract class DatabaseRemoteConfig<T, T1> where T : DatabaseRemoteConfig<T, T1>, new() where T1 : BaseConfig<T1>, new()
    {
        private static Func<DocumentReference> getter;
        private static readonly T instance;
        protected static string UserId { get; private set; }
        
        private static bool ServerEnabled
        {
            get
            {
#if DEBUG
                return Config.serverEnabled;
#else
                return true;
#endif
            }
        }

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
            User.SignIn(() =>
            {
                Burger.Log($"[{typeof(T1).Name}] Push");
                var docRef = getter();
                var config = BaseConfig<T1>.Config;
                var json = JsonConvert.SerializeObject(config, config.Settings);
                var dict = new Dictionary<string, object>()
                {
                    {BaseConfig<T1>.Config.FileName, json}
                };
                var pushTask = docRef.SetAsync(dict);
                
                pushTask.ContinueWithOnMainThread(task =>
                {
                    if (task.IsCompleted && task.IsCompletedSuccessfully)
                    {
                        Burger.Log($"[{typeof(T1).Name}] Success Push");
                        onSuccess.SafeInvoke();
                    }
                    else if (task.IsFaulted || task.IsCanceled)
                    {
                        Burger.Error($"[{typeof(T1).Name}] Failure Push: {task.Exception.Message}");
                        onError.SafeInvoke();
                    }
                });
            }, onError);
        }
        
        public static void Fetch(string userId, Action<T1> onSuccess, Action onError = null, Action onComplete = null)
        {
            if (!ServerEnabled)
            { 
                onSuccess.SafeInvoke(BaseConfig<T1>.Config);
                return; 
            }
            
            Internal_Fetch(userId, onSuccess: dict =>
            {
                var config = BaseConfig<T1>.Config;
                var json = (string) dict[config.FileName];
                config = JsonConvert.DeserializeObject<T1>(json, config.Settings);
                onSuccess.SafeInvoke(config);
            }, onError, onComplete);
        }

        public static void Fetch(Action onSuccess = null, Action onError = null, Action onComplete = null)
        {
            if (!ServerEnabled)
            { 
                onSuccess.SafeInvoke();
                return; 
            }
            
            OnAppPause.UnSubscribe(OnApplicationPause);
            OnAppPause.Subscribe(OnApplicationPause);
            Internal_Fetch(User.Id, onSuccess: dict =>
            {
                var config = BaseConfig<T1>.Config;
                var json = (string) dict[config.FileName];
                BaseConfig<T1>.Deserialize(json);
                onSuccess.SafeInvoke();
            }, onError, onComplete, () => Push(onSuccess, onError));
        }

        private static void Internal_Fetch(string userId, Action<Dictionary<string, object>> onSuccess = null,
            Action onError = null,
            Action onComplete = null,
            Action onResponseEmpty = null)
        {
            User.SignIn(() =>
            {
                Burger.Log($"[{typeof(T1).Name}] Fetch");

                if (string.IsNullOrEmpty(userId))
                {
                    Debug.LogWarning($"[{typeof(T1).Name}] UserId is Null or Empty!");
                    userId = User.Id;
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
                                Burger.Log($"[{typeof(T1).Name}] Failure Fetch: Response is empty. User: {userId}");
                                onResponseEmpty.SafeInvoke();
                            }
                            else
                            {
                                Burger.Error($"[{typeof(T1).Name}] Failure Fetch: Response is empty. User: {userId}");
                            }
                            
                            onError.SafeInvoke();
                            onComplete.SafeInvoke();
                            return;
                        }
                        
                        Burger.Log($"[{typeof(T1).Name}] Success Fetch. User: {userId}"); 
                        onSuccess.SafeInvoke(dict);
                        onComplete.SafeInvoke();
                    }
                    else
                    {
                        Burger.Error($"[{typeof(T1).Name}] Failure Fetch {task.Exception.Message}. User: {userId}");
                        onError.SafeInvoke();
                        onComplete.SafeInvoke();
                    }
                });
            }, onError);
        }

        private static void OnApplicationPause()
        {
            Push();
        }
        
        private static void Invoke(Action action)
        {
            try { action?.Invoke(); }
            catch (Exception e) { Debug.LogException(e); }
        }
    }
}