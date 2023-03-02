using Battle.Data;
using BeatRoyale.Windows;
using Core.ConfigModule;
using Core.SingleService;
using Firebase.Auth;
using Firebase.Extensions;
using UnityEngine;
using static Battle.Data.LevelsConfigsManager;

namespace BeatRoyale.Launcher
{
    public partial class LauncherWorld : ServiceManager
    {
        [SerializeField] private BackgroundAnimationData backgroundData;
        [SerializeField] private LevelsConfigsManager levelsConfigsManager;

        protected override void Awake()
        {
            base.Awake();
            Application.targetFrameRate = 60;
            ControlPanel.Show();
        }

        private void Start()
        {
            FirebaseAuth.DefaultInstance.SignInAnonymouslyAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsCompletedSuccessfully)
                {
                    StorageRemoteConfig<ChangedLevels>.Fetch(Init);
                }
                else
                {
                    Debug.Log($"[{nameof(LauncherWorld)}] {task.Exception.Message}");
                }
            });
        }

        private void Init()
        {
            levelsConfigsManager.Init();
            LevelUpgraded += LogAllProperties;
            LogAllProperties();
        }

        private void LogAllProperties()
        {
            foreach (var level in EntitiesProperties.Config.Properties)
            {
                foreach (var prop in level.Value)
                {
                    var value = ((float) prop.Value.value); 
                    Debug.Log($"[Malvis] Scope: {level.Key}, Property Type: {prop.Key}," +
                        $" Value: {prop.Value.value}, Percent: {prop.Value.percent}%, Total: {value + value * (prop.Value.percent / 100)}");
                }
            }
        }

        private void Update()
        {
            backgroundData.Update();
        }
    }
}

