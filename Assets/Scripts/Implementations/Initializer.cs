using System.Collections.Generic;
using System.Linq;
using Battle.Data;
using StarSavers.Data;
using StarSavers.Interfaces;
using DG.Tweening;
using LSCore;
using LSCore.BattleModule;
using LSCore.ConfigModule;
using LSCore.LevelSystem;
using Sirenix.OdinInspector;
using UnityEngine;
using Action = System.Action;

namespace StarSavers
{
    [ShowOdinSerializedPropertiesInInspector]
    public class Initializer : BaseInitializer
    {
        [Id("Heroes")] [SerializeField] private List<Id> firstUnlockedHeroes;
        
        [SerializeField] private LevelsManager heroesLevelsManager;
        [SerializeField] private ExchangeTable exchangeTable;
        [SerializeField] private HeroRankIconsConfigs heroRankIconsConfigs;

        [SerializeField] private Funds funds;

        static Initializer()
        {
            World.Destroyed += () => isInited = false;
        }
        
        private static bool isInited;

        protected override void Internal_Initialize(Action onInit)
        {
            DontDestroyOnLoad(gameObject);
            if (isInited)
            {
                onInit();
                return;
            }
            
            isInited = true;
            
#if UNITY_EDITOR
            Application.targetFrameRate = 1000;
#else
            Application.targetFrameRate = 120;
#endif
            DOTween.SetTweensCapacity(200, 200);
            
            heroesLevelsManager.Init();
            exchangeTable.Init();
            heroRankIconsConfigs.Init();

            if (string.IsNullOrEmpty(PlayerData.Config.SelectedHero.Value))
            {
                PlayerData.Config.SelectedHero.Value = firstUnlockedHeroes[0];
            }
            
            Id selectedHeroId = PlayerData.Config.SelectedHero.Value;
            
            if (!heroesLevelsManager.AddedIds.Contains(selectedHeroId))
            {
                selectedHeroId = heroesLevelsManager.AddedIds.First();
                PlayerData.Config.SelectedHero.Value = selectedHeroId;
            }
            
            const string ftKey = "Give funds and hero";
            if (FirstTime.IsNot(ftKey, out var pass))
            {
                funds.Earn();
                for (int i = 0; i < firstUnlockedHeroes.Count; i++)
                {
                    heroesLevelsManager.UpgradeLevel(firstUnlockedHeroes[i]);
                }
            
                pass();
            }
            else if (!UnlockedLevels.TryGetLevel(selectedHeroId, out _))
            {
                heroesLevelsManager.SetLevel(selectedHeroId, 1);
            }
            
            var unit = heroesLevelsManager.GetCurrentLevel<Unit>(selectedHeroId);
            unit.RegisterComps();
            Debug.Log(unit.GetComp<BaseHealthComp>().Health);
            
            onInit();
        }
    }
}