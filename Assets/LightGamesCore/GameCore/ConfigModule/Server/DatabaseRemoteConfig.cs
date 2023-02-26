using System;
using System.Collections.Generic;
using Firebase.Extensions;
using Firebase.Firestore;
using Newtonsoft.Json;
using UnityEngine;

namespace Core.ConfigModule
{
    public abstract class DatabaseRemoteConfig<T, T1> where T : DatabaseRemoteConfig<T, T1>, new() where T1 : BaseConfig<T1>, new()
    {
        private const int MaxAttemption = 1;
        private static DocumentReference reference;
        private static Func<DocumentReference> getter;
        private static bool isOnRemote;
        private static int attemption;
        private static T instance;

        static DatabaseRemoteConfig()
        {
            instance = new T();
            getter = GetReference;
            UnityEventWrapper.SubscribeToApplicationPausedEvent(OnApplicationPause);
        }

        private static DocumentReference GetReference()
        {
            getter = GetCreatedReference;
            reference = instance.Reference;

            return reference;
        }

        protected abstract DocumentReference Reference { get; }
        private static DocumentReference GetCreatedReference() => reference;

        public static void Push(Action onSuccess = null, Action onError = null)
        {
            if (!isOnRemote)
            {
                if (attemption < MaxAttemption)
                {
                    attemption++;
                    CheckIsOnRemote(onComplete: () => Push(onSuccess, onError));
                    return;
                }
            }

            Debug.Log($"[{typeof(T).Name}<{typeof(T1).Name}>] Push {isOnRemote}");
            attemption = 0;
            var docRef = getter();
            var config = BaseConfig<T1>.Config;
            var json = JsonConvert.SerializeObject(config, config.Settings);
            var dict = JsonConvert.DeserializeObject<Dictionary<string, object>>(json, config.Settings);
            var pushTask = isOnRemote ? docRef.UpdateAsync(dict) : docRef.SetAsync(dict);
            
            pushTask.ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted && task.IsCompletedSuccessfully)
                {
                    isOnRemote = true;
                    Debug.Log($"[{typeof(T).Name}<{typeof(T1).Name}>] Push onSuccess {isOnRemote}");
                    onSuccess?.Invoke();
                }
                else if (task.IsFaulted || task.IsCanceled)
                {
                    Debug.Log($"[{typeof(T).Name}<{typeof(T1).Name}>] Push onError {isOnRemote}");
                    onError?.Invoke();
                }
            });
        }

        public static void Fetch(Action onSuccess = null, Action onError = null, Action onComplete = null)
        {
            CheckIsOnRemote(onSuccess: dict =>
            {
                var json = JsonConvert.SerializeObject(dict, BaseConfig<T1>.Config.Settings);
                BaseConfig<T1>.Deserialize(json);
                onSuccess?.Invoke();
            }, onError, onComplete);
        }

        private static void CheckIsOnRemote(Action<Dictionary<string, object>> onSuccess = null, Action onError = null, Action onComplete = null)
        {
            var docRef = getter();

            docRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted && task.IsCompletedSuccessfully)
                {
                    var dict = task.Result.ToDictionary();

                    if (dict == null)
                    {
                        isOnRemote = false;
                        Debug.Log($"[{typeof(T).Name}<{typeof(T1).Name}>] CheckIsOnRemote onError {isOnRemote} {attemption}");
                        onError?.Invoke();
                        onComplete?.Invoke();
                        return;
                    }
                    
                    isOnRemote = true;
                    Debug.Log($"[{typeof(T).Name}<{typeof(T1).Name}>] CheckIsOnRemote onSuccess {isOnRemote} {attemption}");
                    onSuccess?.Invoke(dict);
                    onComplete?.Invoke();
                }
                else if (task.IsFaulted || task.IsCanceled)
                {
                    isOnRemote = false;
                    Debug.Log($"[{typeof(T).Name}<{typeof(T1).Name}>] CheckIsOnRemote onError {isOnRemote} {attemption}");
                    onError?.Invoke();
                    onComplete?.Invoke();
                }
            });
        }

        private static void OnApplicationPause()
        {
            Push();
        }
    }
}