using System;
using Battle.Data;
using Common.SingleServices;
using Core.ConfigModule;
using Core.Server;
using GameCore.Battle.Data;
using Newtonsoft.Json.Utilities;
using UnityEngine;

namespace BeatRoyale.Launcher
{
    public partial class LauncherWorld
    {
        private static bool isInited;

        private void Initialize(Action onInit)
        {
            if (isInited)
            {
                onInit();
                return;
            }
            
            isInited = true;

            AotHelper.EnsureList<LongNoteData>();
            AotHelper.EnsureList<ShortNoteData>();
#if !UNITY_EDITOR
            Application.targetFrameRate = 60;
#else
            Application.targetFrameRate = 1000;
#endif
            var loader = Loader.Create();
            onInit += loader.Destroy;
            
            RemotePlayerData<User>.Fetch(() =>
            {
                RemotePlayerData<Leaderboards>.Fetch(() =>
                {
                    StorageRemoteConfig<ChangedLevels>.Fetch(() =>
                    {
                        RemotePlayerData<UnlockedLevels>.Fetch(() =>
                        {
                            RemotePlayerData<EntitiesProperties>.Fetch(() =>
                            {
                                levelsConfigsManager.Init();
                                RemotePlayerData<CardDecks>.Fetch(onInit);
                            });
                        });
                    });
                });
            });
        }
    }
}