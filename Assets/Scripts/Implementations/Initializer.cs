using System;
using System.Linq;
using Battle.Data;
using BeatHeroes.Data;
using BeatHeroes.Interfaces;
using DG.Tweening;
using LSCore;
using LSCore.ConfigModule;
using LSCore.LevelSystem;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using static LSCore.UnitySerializationUtils;

namespace BeatHeroes
{
    [ShowOdinSerializedPropertiesInInspector]
    public class Initializer : BaseInitializer, ISerializationCallbackReceiver, ISupportsPrefabSerialization
    {
        [SerializeField] private LevelsManager heroesLevelsManager;
        [SerializeField] private LevelsManager enemiesLevelsManager;
        [SerializeField] private ExchangeTable exchangeTable;
        [SerializeField] private HeroRankIconsConfigs heroRankIconsConfigs;
        [SerializeField] private Palette palette;
        
        [Id("Heroes")] [SerializeField] private Id[] ids;
        [Id("Enemies")] [SerializeField] private Id[] enemyIds;
        [OdinSerialize] private Funds funds;

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
            DOTween.SetTweensCapacity(200, 200);

            Id selectedHeroId = PlayerData.Config.SelectedHero;
            if (!heroesLevelsManager.Group.Contains(selectedHeroId))
            {
                selectedHeroId = heroesLevelsManager.Group.First();
                PlayerData.Config.SelectedHero = selectedHeroId;
            }
            
            heroesLevelsManager.Init();
            enemiesLevelsManager.Init();
            exchangeTable.Init();
            heroRankIconsConfigs.Init();
            palette.Init();
            
            const string ftKey = "Give funds and hero";
            
            if (FirstTime.IsNot(ftKey, out var pass))
            {
                funds.Earn();
                for (int i = 0; i < ids.Length; i++)
                {
                    heroesLevelsManager.UpgradeLevel(ids[i]);
                }
                
                for (int i = 0; i < enemyIds.Length; i++)
                {
                    enemiesLevelsManager.UpgradeLevel(enemyIds[i]);
                }
            
                pass();
            }
            else if (!UnlockedLevels.TryGetLevel(selectedHeroId, out var level))
            {
                heroesLevelsManager.SetLevel(selectedHeroId, 1);
            }
            
            onInit();
        }
        
        
        [SerializeField, HideInInspector] private SerializationData serializationData;
        SerializationData ISupportsPrefabSerialization.SerializationData { get => serializationData; set => serializationData = value; }
        void ISerializationCallbackReceiver.OnAfterDeserialize() => Deserialize(this, ref serializationData);
        void ISerializationCallbackReceiver.OnBeforeSerialize() => Serialize(this, ref serializationData);
    }
}