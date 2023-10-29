using System;
using BeatHeroes.Interfaces;
using DG.Tweening;
using LSCore;
using LSCore.LevelSystem;
using UnityEngine;

namespace BeatHeroes
{
    public class Initializer : BaseInitializer
    {
        [SerializeField] private LevelsManager levelsManager;
        
        [Id("Heroes")] [SerializeField] private Id[] ids;

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
            levelsManager.Init();
            DOTween.SetTweensCapacity(200, 200);
            
            for (int i = 0; i < ids.Length; i++)
            {
                levelsManager.UpgradeLevel(ids[i]);
            }
            
            onInit();
        }
    }
}