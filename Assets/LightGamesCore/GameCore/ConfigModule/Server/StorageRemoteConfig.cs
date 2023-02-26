using System;
using System.Text;
using Firebase.Extensions;
using Firebase.Storage;
using Newtonsoft.Json;
using UnityEngine;

namespace Core.ConfigModule
{
    public static class StorageRemoteConfig<T> where T : BaseConfig<T>, new()
    {
        private const long MaxAllowedSize = 4 * 1024 * 1024;
        private static StorageReference reference;
        private static Func<StorageReference> getter;

        static StorageRemoteConfig()
        {
            getter = GetReference;
        }

        private static StorageReference GetReference()
        {
            getter = GetCreatedReference;
            var config = BaseConfig<T>.Config;
            reference = FirebaseStorage.DefaultInstance.RootReference.Child($"{FolderNames.Configs}/{config.FileName}.{config.Ext}");

            return reference;
        }
        private static StorageReference GetCreatedReference() => reference;

        public static void Push(Action onSuccess = null, Action onError = null)
        {
            var storageRef = getter();
            Debug.Log($"[{typeof(T).Name}<{typeof(T).Name}>] Push {storageRef.Path}");
            var config = BaseConfig<T>.Config;
            var json = JsonConvert.SerializeObject(config, config.Settings);
            storageRef.PutBytesAsync(Encoding.UTF8.GetBytes(json)).ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted && task.IsCompletedSuccessfully)
                {
                    Debug.Log($"[{typeof(T).Name}] Push onSuccess");
                    onSuccess?.Invoke();
                }
                else if (task.IsFaulted || task.IsCanceled)
                {
                    Debug.Log($"[{typeof(T).Name}] Push onError {task.Exception.Message}");
                    onError?.Invoke();
                }
            });
        }

        public static void Fetch(Action onSuccess = null, Action onError = null)
        {
            var storRef = getter();
            Debug.Log($"[{typeof(T).Name}] Fetch {storRef.Path}");
            
            getter().GetBytesAsync(MaxAllowedSize).ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted && task.IsCompletedSuccessfully)
                {
                    var bytes = task.Result;
                    
                    if (bytes == null)
                    {
                        Debug.Log($"[{typeof(T).Name}] Fetch onError {task.Exception.Message}");
                        onError?.Invoke();
                        return;
                    }
                    
                    BaseConfig<T>.Deserialize(Encoding.UTF8.GetString(bytes));
                    Debug.Log($"[{typeof(T).Name}] Fetch onSuccess");
                    onSuccess?.Invoke();
                }
                else if (task.IsFaulted || task.IsCanceled)
                {
                    Debug.Log($"[{typeof(T).Name}] Fetch onError {task.Exception.Message}");
                    onError?.Invoke();
                }
            });
        }
    }
}