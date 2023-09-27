using System;
using Battle.Data;
using BeatRoyale.Interfaces;
using UnityEngine;

namespace BeatRoyale
{
    public class Initializer : BaseInitializer<IInitializer>
    {
        [SerializeField] private LevelsConfigsManager levelsConfigsManager;
        private static bool isInited;

        protected override void Internal_Initialize(Action onInit)
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
            levelsConfigsManager.Init();
            onInit();
        }
    }
}