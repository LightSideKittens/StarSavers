#if !UNITY_EDITOR
#define NOT_EDITOR
#endif

using System;
using System.Diagnostics;
using UnityEngine;

namespace Core.ConfigModule
{
    public class UnityEventWrapper : MonoBehaviour
    {
        private static event Action ApplicationPaused;
        private static bool isCreated;

        private void Awake()
        {
            DontDestroyOnLoad(this);
        }

        [Conditional("UNITY_EDITOR")]
        private static void Editor_CanCreate(ref bool canCreate) => canCreate = !isCreated && Application.isPlaying;
        
        [Conditional("NOT_EDITOR")]
        private static void Runtime_CanCreate(ref bool canCreate) => canCreate = !isCreated;

        public static void SubscribeToApplicationPausedEvent(Action action)
        {
            bool canCreated = false;
            Editor_CanCreate(ref canCreated);
            Runtime_CanCreate(ref canCreated);
            
            if (canCreated)
            { 
                new GameObject(nameof(UnityEventWrapper)).AddComponent<UnityEventWrapper>();
                isCreated = true;
            }
            
            ApplicationPaused += action;
        }
        
        public static void UnSubscribeFromApplicationPausedEvent(Action action)
        {
            ApplicationPaused -= action;
        }

        private void OnDisable()
        {
            InitConfigTypeData();
        }

        [Conditional("UNITY_EDITOR")]
        private static void InitConfigTypeData()
        {
            ConfigsTypeData.Init();
            ConfigsTypeData.Save();
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus)
            {
                ApplicationPaused?.Invoke();
            }
        }

        private void OnApplicationQuit()
        {
            OnApplicationPause(true);
        }
    }
}