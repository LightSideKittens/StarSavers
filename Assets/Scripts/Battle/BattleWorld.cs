using System.Collections.Generic;
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
        private Dictionary<Id, int> funds = new();
        
        public static Dictionary<Id, int> Funds => Instance.funds;
        public static RaidByHeroRank Raids => Instance.raids;
        public static RaidConfig Raid => Instance.raids.Current;
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

            Unit.Killed += OnUnitKilled;
            PlayerWorld.Begin();
            OpponentWorld.Begin();
            BattleWindow.Show();
            funds.Clear();
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
            Unit.Killed -= OnUnitKilled;
            World.Updated -= UpdateTime;
            OpponentWorld.AllUnitsDestroyed -= StartBreak;
        }

        private static void StartWave()
        {
            World.Updated += UpdateTime;
            OpponentWorld.Continue();
            Wait.Delay(Raid.GetWaveDuration(), PauseWave);
            Raid.CurrentWave++;
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
            Wait.Delay(Raid.BreakDuration, StartWave);
        }
        
        public static Id GetEnemyId() => Raid.GetEnemyId((int)TimeSinceStart);
        private static void OnUnitKilled(Unit unit) => Raid.OnEnemyKilled(unit.Id);
        public static float GetSpawnFrequency() => Raid.GetSpawnFrequency((int)TimeSinceStart);
    }
}