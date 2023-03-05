using Battle.Data;
using BeatRoyale.Windows;
using Core.ConfigModule;
using Core.SingleService;
using Firebase.Auth;
using Firebase.Extensions;
using GameCore.Attributes;
using GameCore.Battle.Data;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Battle.Data.LevelsConfigsManager;

namespace BeatRoyale.Launcher
{
    public class LauncherWorld : ServiceManager
    {
        [ColoredField, SerializeField] private CastleBackAnimator animator;
        [SerializeField] private LevelsConfigsManager levelsConfigsManager;

        protected override void Awake()
        {
            base.Awake();
            Application.targetFrameRate = 60;
        }

        private void Start()
        {
            Auth.SignIn(() =>
            {
                StorageRemoteConfig<ChangedLevels>.Fetch(() =>
                {
                    Debug.Log($"[Malvis] RemotePlayerData<UnlockedLevels>.Fetch");
                    RemotePlayerData<UnlockedLevels>.Fetch(() =>
                    {
                        levelsConfigsManager.Init();
                        LevelUpgraded += LogAllProperties;
                        LogAllProperties();
                        RemotePlayerData<EntitiesProperties>.Fetch(() =>
                        {
                            RemotePlayerData<CardDecks>.Fetch(Init);
                        });
                    });
                });
            });
        }

        private void Init()
        {
            animator.Init();
            ControlPanel.Show();
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
            animator.Update();
        }
    }
}

