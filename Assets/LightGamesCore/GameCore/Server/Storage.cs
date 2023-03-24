using System;
using System.Text;
using System.Threading.Tasks;
using Core.Server;
using Firebase.Extensions;
using Firebase.Storage;
using Newtonsoft.Json;
using static Core.ConfigModule.ConfigVersions;
using static Core.Server.User;

namespace Core.ConfigModule
{
    public static class Storage<T> where T : BaseConfig<T>, new()
    {
        private static StorageReference reference;
        
        private static Func<StorageReference> getter;

        static Storage()
        {
            getter = GetReference;
        }

        private static StorageReference GetReference()
        {
            var configsRef = Storage.RootReference.Child($"{FolderNames.Configs}");
            getter = GetCreatedReference;
            var config = BaseConfig<T>.Config;
            reference = configsRef.Child($"{config.FileName}.{config.Ext}");

            if (versionsReference == null)
            {
                var configVersions = BaseConfig<ConfigVersions>.Config;
                versionsReference = configsRef.Child($"{configVersions.FileName}.{configVersions.Ext}");
            }

            return reference;
        }
        
        private static StorageReference GetCreatedReference() => reference;

        public static void Push(Action onSuccess = null, Action onError = null)
        {
            if (!ServerEnabled)
            { 
                onSuccess?.Invoke();
                return; 
            }

            Internal_Push(onSuccess, onError);
        }

        public static void Fetch(Action onSuccess = null, Action onError = null)
        {
            if (!ServerEnabled)
            { 
                BaseConfig<T>.LoadAsDefault();
                onSuccess?.Invoke();
                return; 
            }
            
            var storageRef = getter();

            if (isCooldown)
            {
                FetchIfNeed();
                return;
            }

            Task.Run(async () =>
            {
                isCooldown = true;
                await Task.Delay(Cooldown);
                isCooldown = false;
            });
            
            Internal_Fetch<ConfigVersions>(versionsReference, OnSuccess, onError);
            
            void OnSuccess(byte[] data)
            {
                var json = Encoding.UTF8.GetString(data);
                SetRemote(json);
                DelegateExtensions.SafeInvoke(FetchIfNeed);
            }

            void FetchIfNeed()
            {
                if (!Compare<T>())
                {
                    Burger.Log($"[{typeof(T).Name}] Different Versions! Local: {GetLocal<T>()} | Remote: {GetRemote<T>()}");
                    Internal_Fetch(storageRef, () =>
                    {
                        Update<T>();
                        onSuccess.SafeInvoke();
                    }, onError);
                }
                else
                {
                    Burger.Log($"[{typeof(T).Name}] Versions is identical! Version: {GetLocal<T>()}");
                    onSuccess.SafeInvoke();
                }
            }
        }

        private static void Internal_Fetch(StorageReference storageRef, Action onSuccess = null, Action onError = null)
        {
            Internal_Fetch<T>(storageRef, data =>
            {
                BaseConfig<T>.Deserialize(Encoding.UTF8.GetString(data));
                onSuccess.SafeInvoke();
            }, onError);
        }

        private static void Internal_Fetch<T1>(StorageReference storageRef, Action<byte[]> onSuccess = null, Action onError = null) where T1 : BaseConfig<T1>, new()
        {
            SignIn(() =>
            {
                Burger.Log($"[{typeof(T1).Name}] Fetch! Path: {storageRef.Path}");
                
                storageRef.GetBytesAsync(MaxAllowedSize).ContinueWithOnMainThread(task =>
                {
                    if (task.IsCompletedSuccessfully)
                    {
                        var bytes = task.Result;
                        
                        if (bytes == null)
                        {
                            Burger.Error($"[{typeof(T1).Name}] Fetch Error: {task.Exception.Message}");
                            onError.SafeInvoke();
                            return;
                        }
                        
                        Burger.Log($"[{typeof(T1).Name}] Fetch Success!");
                        onSuccess.SafeInvoke(bytes);
                    }
                    else
                    {
                        Burger.Error($"[{typeof(T1).Name}] Fetch Error: {task.Exception.Message}");
                        onError.SafeInvoke();
                    }
                });
            }, onError);
        }
        
        private static void Internal_Push(Action onSuccess, Action onError)
        {
            SignIn(() =>
            {
                var storageRef = getter();
            
                var config = BaseConfig<T>.Config;
                var json = JsonConvert.SerializeObject(config, config.Settings);
                storageRef.PutBytesAsync(Encoding.UTF8.GetBytes(json)).ContinueWithOnMainThread(task =>
                {
                    if (task.IsCompletedSuccessfully)
                    {
                        Burger.Log($"[{typeof(T).Name}] Push Success!"); ;
                        onSuccess.SafeInvoke();
                    }
                    else
                    {
                        Burger.Error($"[{typeof(T).Name}] Push Error: {task.Exception.Message}");
                        onError.SafeInvoke();
                    }
                });
            }, onError);
        }
    }
}