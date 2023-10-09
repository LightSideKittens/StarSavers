using System;
using Battle.Data;
using BeatRoyale.Interfaces;
using UnityEngine;
using UnityEngine.Serialization;

namespace BeatRoyale
{
    public class Initializer : BaseInitializer
    {
        [FormerlySerializedAs("levelsConfigsManager")] [SerializeField] private LevelsManager levelsManager;
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
            levelsManager.Init();
            onInit();
        }

        protected override void DeInit()
        {
            isInited = false;
        }
    }
}