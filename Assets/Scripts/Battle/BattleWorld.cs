using Battle.Windows;
using BeatHeroes.Interfaces;
using DG.Tweening;
using Battle.Data;
using LSCore;
using UnityEngine;

namespace Battle
{
    public class BattleWorld : ServiceManager<BattleWorld>
    {
        [SerializeField] private Effectors effectors;
        [SerializeField] private RaidByHeroRank raids;
        public static RaidByHeroRank Raids => Instance.raids;
        public static float TimeSinceStart { get; private set; }
        
        private void Start()
        {
            BaseInitializer.Initialize(OnInitialize);
        }
        
        private void OnInitialize()
        {
            Init();
            enabled = true;
        }

        private void Init()
        {
            effectors.Init();
            raids.Init();
            PlayerWorld.Begin();
            OpponentWorld.Begin();
            BattleWindow.Show();
            TimeSinceStart = 0;
            World.Updated += UpdateTime;
        }

        private static void UpdateTime()
        {
            TimeSinceStart += Time.deltaTime;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            DOTween.KillAll();
            World.Updated -= UpdateTime;
        }

        public static Id GetEnemyId()
        {
            return Raids.Current.GetEnemyId((int)TimeSinceStart);
        }

        public static float GetSpawnFrequency()
        {
            return Raids.Current.GetSpawnFrequency((int)TimeSinceStart);
        }
    }
}