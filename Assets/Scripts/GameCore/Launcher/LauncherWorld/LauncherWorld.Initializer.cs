using System;
using Battle.Data;
using Core.ConfigModule;
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
            RemotePlayerData<CommonPlayerData>.Fetch(() =>
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
        }
    }
}