using System;
using System.Collections.Generic;
using System.Diagnostics;
using Battle.Data;
using BeatHeroes.Interfaces;
using Sirenix.OdinInspector;
using UnityEngine;

namespace BeatHeroes
{
    public class Initializer : BaseInitializer
    {
        [SerializeField] private LevelsManager levelsManager;
        [SerializeField] private EntityMeta entityMeta;
        
        [ValueDropdown("Entities")]
        [SerializeField] private int[] ids;
        
#if UNITY_EDITOR
        private static IList<ValueDropdownItem<int>> Entities => EntityMeta.EntityIds.GetValues();
#endif
        
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
            
            for (int i = 0; i < ids.Length; i++)
            {
                LevelsManager.UpgradeLevel(ids[i]);
            }
            
            onInit();
        }

        protected override void DeInit()
        {
            isInited = false;
        }
    }
}