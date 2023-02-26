using System;
using System.Collections.Generic;
using Battle.Data;
using BeatRoyale.Windows;
using Core.ConfigModule;
using Core.SingleService;
using Firebase.Auth;
using Firebase.Extensions;
using GameCore.Attributes;
using UnityEngine;
using static Battle.Data.LevelsConfigsManager;

namespace BeatRoyale.Launcher
{
    public partial class LauncherWorld : ServiceManager
    {
        [ColoredField, SerializeField] private BackgroundAnimationData backgroundData;
        [SerializeField] private LevelsConfigsManager levelsConfigsManager;
        private readonly Dictionary<string, (LevelConfig[], Dictionary<Type, float>)> levels = new();
        [SerializeField] private TextAsset textAsset;

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
            UpgradeLevel("Dumbledore", 2);
            UpgradeLevel("Dumbledore", 3);
            UpgradeLevel("Raccoon", 1);
            UpgradeLevel("Raccoon", 2);
            UpgradeLevel("Raccoon", 3);
            UpgradeLevel("Witch", 3);
            UpgradeLevel("Witch", 1);
            UpgradeLevel("Witch", 2);
            UpgradeLevel("Witch", 1);
            UpgradeLevel("WrongEntitiesScope", 1);
        }

        private void LogAllProperties()
        {
            foreach (var level in EntitiesProperties.Config.Properties)
            {
                foreach (var prop in level.Value)
                {
                    Debug.Log($"[Malvis] Scope: {level.Key}, Property Type: {prop.Key}," +
                        $" Value: {prop.Value.value}, Percent: {prop.Value.percent * 100}%, Total: {prop.Value.value + prop.Value.value * prop.Value.percent}");
                }
            }
        }

        private void Update()
        {
            backgroundData.Update();
        }
    }
}

