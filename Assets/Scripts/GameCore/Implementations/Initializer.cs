using System;
using System.Collections.Generic;
using System.Diagnostics;
using Battle.Data;
using BeatHeroes.Interfaces;
using DG.Tweening;
using LSCore;
using Sirenix.OdinInspector;
using UnityEngine;

namespace BeatHeroes
{
    public class Initializer : BaseInitializer
    {
        [SerializeField] private LevelsManager levelsManager;
        [SerializeField] private EntityMeta entityMeta;
        
        [EntityId] [SerializeField] private int[] ids;

        static Initializer()
        {
            World.Destroyed += () => isInited = false;
        }
        
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
            DOTween.SetTweensCapacity(200, 200);
            
            for (int i = 0; i < ids.Length; i++)
            {
                LevelsManager.UpgradeLevel(ids[i]);
            }
            
            onInit();
        }
    }
}