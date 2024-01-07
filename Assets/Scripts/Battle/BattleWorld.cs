using Battle.Windows;
using BeatHeroes.Interfaces;
using DG.Tweening;
using Battle.Data;
using LSCore;
using LSCore.Async;
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
            StartWave();
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
            OpponentWorld.AllUnitsDestroyed -= StartBreak;
        }

        private static void StartWave()
        {
            World.Updated += UpdateTime;
            OpponentWorld.Continue();
            Wait.Delay(Raids.Current.GetWaveDuration(), PauseWave);
            Raids.Current.CurrentWave++;
        }

        private static void PauseWave()
        {
            World.Updated -= UpdateTime;
            OpponentWorld.Pause();
                
            if (OpponentWorld.UnitCount == 0)
            {
                StartBreak();
            }
            else
            {
                OpponentWorld.AllUnitsDestroyed += StartBreak;
            }
        }

        private static void StartBreak()
        {
            OpponentWorld.AllUnitsDestroyed -= StartBreak;
            Wait.Delay(Raids.Current.BreakDuration, StartWave);
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