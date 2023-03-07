using Battle.Data;
using BeatRoyale.Windows;
using Core.ConfigModule;
using Core.SingleService;
using GameCore.Attributes;
using GameCore.Battle.Data;
using Newtonsoft.Json.Utilities;
using UnityEngine;

namespace BeatRoyale.Launcher
{
    public class LauncherWorld : ServiceManager
    {
        [ColoredField, SerializeField] private CastleBackAnimator animator;
        [SerializeField] private LevelsConfigsManager levelsConfigsManager;

        protected override void Awake()
        {
            base.Awake();
            AotHelper.EnsureList<LongNoteData>();
            AotHelper.EnsureList<ShortNoteData>();
#if !UNITY_EDITOR
            Application.targetFrameRate = 60;
#else
            Application.targetFrameRate = 1000;
#endif
        }

        private void Start()
        {
            Auth.SignIn(() =>
            {
                StorageRemoteConfig<ChangedLevels>.Fetch(() =>
                {
                    RemotePlayerData<UnlockedLevels>.Fetch(() =>
                    {
                        levelsConfigsManager.Init();
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
        
        private void Update()
        {
            animator.Update();
        }
    }
}

