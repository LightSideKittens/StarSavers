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
        [SerializeField] private LevelsManager heroesLevelsManager;
        [SerializeField] private ExchangeTable exchangeTable;
        [SerializeField] private HeroRankIconsConfigs heroRankIconsConfigs;
        
        [Id("Heroes")] [SerializeField] private Id[] ids;
        [Id("Enemies")] [SerializeField] private Id[] enemyIds;
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
            
            Id selectedHeroId = PlayerData.Config.SelectedHero;
            if (!heroesLevelsManager.AddedIds.Contains(selectedHeroId))
            {
                selectedHeroId = heroesLevelsManager.AddedIds.First();
                PlayerData.Config.SelectedHero = selectedHeroId;
            }
            
            const string ftKey = "Give funds and hero";
            if (FirstTime.IsNot(ftKey, out var pass))
            {
                funds.Earn();
                for (int i = 0; i < ids.Length; i++)
                {
                    heroesLevelsManager.UpgradeLevel(ids[i]);
                }
            
                pass();
            }
            else if (!UnlockedLevels.TryGetLevel(selectedHeroId, out var level))
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