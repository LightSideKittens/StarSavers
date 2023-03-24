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
            
#if UNITY_EDITOR
            Application.targetFrameRate = 1000;
#else
            Application.targetFrameRate = 60;
#endif
            var loader = Loader.Create();
            onInit += loader.Destroy;
            
            UserDatabase<User>.Fetch(() =>
            {
                UserDatabase<Leaderboards>.Fetch(() =>
                {
                    UserDatabase<UnlockedLevels>.Fetch(() =>
                    {
                        UserDatabase<EntiProps>.Fetch(() =>
                        {
                            if (ConfigVersions.RemoteCount == 0)
                            {
                                Storage<ConfigVersions>.Fetch(OnComplete);
                            }
                            else
                            {
                                OnComplete();
                            }
                            
                            void OnComplete()
                            {
                                levelsConfigsManager.Init();
                                UserDatabase<CardDecks>.Fetch(onInit);
                            }
                        });
                    });
                });
            });
        }
    }
}