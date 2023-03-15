using System;
using System.Collections.Generic;
using BeatRoyale;
using Firebase.Auth;
using Firebase.Extensions;
using Firebase.Firestore;
using Newtonsoft.Json;
using UnityEngine;

namespace Core.ConfigModule
{
    public abstract class DatabaseRemoteConfig<T, T1> where T : DatabaseRemoteConfig<T, T1>, new() where T1 : BaseConfig<T1>, new()
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
            Debug.Log($"[{typeof(T1).Name}] Push");
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
                    Debug.Log($"[{typeof(T1).Name}] Success Push");
                    Invoke(onSuccess);
                }
                else if (task.IsFaulted || task.IsCanceled)
                {
                    Debug.LogError($"[{typeof(T1).Name}] Failure Push: {task.Exception.Message}");
                    Invoke(onError);
                }
            });
        }
        
        public static void Fetch(string userId, Action<T1> onSuccess, Action onError = null, Action onComplete = null)
        {
            Internal_Fetch(userId, onSuccess: dict =>
            {
                var config = BaseConfig<T1>.Config;
                var json = (string) dict[config.FileName];
                config = JsonConvert.DeserializeObject<T1>(json, config.Settings);
                Invoke(() => onSuccess?.Invoke(config));
            }, onError, onComplete);
        }

        public static void Fetch(Action onSuccess = null, Action onError = null, Action onComplete = null)
        {
            OnAppPause.UnSubscribe(OnApplicationPause);
            OnAppPause.Subscribe(OnApplicationPause);
            Internal_Fetch(CommonPlayerData.UserId, onSuccess: dict =>
            {
                var config = BaseConfig<T1>.Config;
                var json = (string) dict[config.FileName];
                BaseConfig<T1>.Deserialize(json);
                Invoke(onSuccess);
            }, onError, onComplete, () => Push(onSuccess, onError));
        }

        private static void Internal_Fetch(string userId, Action<Dictionary<string, object>> onSuccess = null,
            Action onError = null,
            Action onComplete = null,
            Action onResponseEmpty = null)
        {
            Debug.Log($"[{typeof(T1).Name}] Fetch");
            UserId = userId;
            var docRef = getter();

            docRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted && task.IsCompletedSuccessfully)
                {
                    var dict = task.Result.ToDictionary();

                    if (dict == null)
                    {
                        if (onResponseEmpty != null)
                        {
                            Debug.Log($"[{typeof(T1).Name}] Failure Fetch: Response is empty. User: {userId}");
                            onResponseEmpty();
                        }
                        else
                        {
                            Debug.LogError($"[{typeof(T1).Name}] Failure Fetch: Response is empty. User: {userId}");
                        }
                        
                        Invoke(onError);
                        Invoke(onComplete);
                        return;
                    }
                    
                    Debug.Log($"[{typeof(T1).Name}] Success Fetch. User: {userId}");
                    Invoke(() => onSuccess?.Invoke(dict));
                    Invoke(onComplete);
                }
                else if (task.IsFaulted || task.IsCanceled)
                {
                    Debug.LogError($"[{typeof(T1).Name}] Failure Fetch {task.Exception.Message}. User: {userId}");
                    Invoke(onError);
                    Invoke(onComplete);
                }
            });
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