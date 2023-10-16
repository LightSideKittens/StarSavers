using System;
using Battle.Data;
using BeatHeroes.Interfaces;
using UnityEngine;

namespace BeatHeroes
{
    public class Initializer : BaseInitializer
    {
        [SerializeField] private LevelsManager levelsManager;
        [SerializeField] private EntityMeta entityMeta;
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
            entityMeta.Init();
            levelsManager.Init();
            LevelsManager.UpgradeLevel(EntityMeta.EntityIds.GetIdByName("Arcane"));
            onInit();
        }

        protected override void DeInit()
        {
            isInited = false;
        }
    }
}