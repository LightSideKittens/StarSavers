using System;
using System.Collections.Generic;
using Animatable;
using Battle;
using Battle.Windows;
using MultiWars.Interfaces;
using DG.Tweening;
using Battle.Data;
using LSCore.Async;
using LSCore.Extensions.Unity;
using UnityEngine;

namespace LSCore.BattleModule
{
    public class BattleWorld : ServiceManager<BattleWorld>
    {
        [Serializable]
        public class RegularWave : RaidConfig.WaveAction
        {
            public float duration;
            
            public override void OnStart()
            {
                World.Updated += UpdateTime;
                OpponentWorld.Continue();

                currentWave++;
                timeTextPrefix = $"Wave {currentWave}";
                BattleWindow.SplashText($"WAVE {currentWave}");

                Wait.TimerBack(duration, UpdateTimeText).OnComplete(OnWaveCompleted);
            }

            public override void OnComplete()
            {
                base.OnComplete();
                World.Updated -= UpdateTime;
                OpponentWorld.Pause();
            }
        }
        
        [Serializable]
        public class BossWave : RaidConfig.WaveAction
        {
            public RaidConfig.BossData bossData;
            
            public override void OnStart()
            {
                BattleWindow.BossHealth.maxValue = OpponentWorld.GetBossHealth(bossData);
                BattleWindow.IsBossMode = true;
                OpponentWorld.UnleashKraken(bossData, OnWaveCompleted);
            }

            public override void OnComplete()
            {
                base.OnComplete();
                BattleWindow.IsBossMode = false;
            }
        }
        
        [SerializeField] private Effectors effectors;
        [SerializeField] private RaidByHeroRank raids;
        public static RaidConfig Raid => Instance.raids.Current;
        private static float timeSinceStart;
        private static int currentWave;
        private static RaidConfig.WaveData wave;
        private static bool isLastWave;
        public static Camera Camera { get; private set; }
        public static Rect CameraRect { get; private set; }


        protected override void Awake()
        {
            base.Awake();
            Camera = Camera.main;
            CameraRect = Camera.GetRect();
        }

        private void Start()
        {
            BaseInitializer.Initialize(OnInitialize);
        }

        private void OnEnable()
        {
            World.Updated += Run;
        }

        private void OnDisable()
        {
            World.Updated -= Run;
        }

        private void Run()
        {
           CameraRect = Camera.GetRect();
        }

        private void OnInitialize()
        {
            Setup();
            enabled = true;
        }

        private void Setup()
        {
            Physics2DExt.SetHitCollidersSize(100);
            AnimatableCanvas.SortingOrder = WindowsData.DefaultSortingOrder - 1;
            BattleWindow.AsHome();
            BattleWindow.Show();
            
            effectors.Init();
            raids.Setup();
            Unit.Releasedd += OnUnitReleasedd;
            PlayerWorld.Begin();
            OpponentWorld.Begin();
            timeSinceStart = 0;
            currentWave = 0;
            StartWave();
            
        }

        private static void UpdateTime()
        {
            timeSinceStart += Time.deltaTime;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            Raid.Dispose();
            DOTween.KillAll();
            Unit.Releasedd -= OnUnitReleasedd;
            World.Updated -= UpdateTime;
            OpponentWorld.AllUnitsReleased -= StartBreak;
        }

        private static void StartWave()
        {
            wave = Raid.GetWave();
            isLastWave = !Raid.MoveNextWave();
            foreach (var action in wave.actions)
            {
                action.OnStart();
            }
        }

        private static string timeTextPrefix;
        private static int currentTime;
        private static void UpdateTimeText(float time)
        {
            currentTime = (int)time;
            BattleWindow.StatusText.text = $"{timeTextPrefix}: {currentTime}s\nEnemy Count: {OpponentWorld.UnitCount}";
        }
        
        private static void OnWaveCompleted()
        {
            foreach (var action in wave.actions)
            {
                action.OnComplete();
            }
            
            Action onAllUnitKilled = StartBreak;
            if (isLastWave)
            {
                onAllUnitKilled = Win;
            }
            
            if (OpponentWorld.UnitCount == 0)
            {
                onAllUnitKilled();
            }
            else
            {
                OpponentWorld.AllUnitsReleased += onAllUnitKilled;
            }
        }

        private static void Win()
        {
            OpponentWorld.AllUnitsReleased -= Win;
            MatchResultWindow.Show(true);
        }

        private static void StartBreak()
        {
            OpponentWorld.AllUnitsReleased -= StartBreak;
            timeTextPrefix = "Break";
            Wait.TimerBack(Raid.BreakDuration, UpdateTimeText).OnComplete(StartWave);
        }
        
        public static Id GetEnemyId() => Raid.GetEnemyId((int)timeSinceStart);
        public static IEnumerable<Id> EnemyIds => Raid.EnemyIds;
        private static void OnUnitReleasedd(Unit unit)
        {
            UpdateTimeText(currentTime);
            Raid.OnEnemyKilled(unit.Id);
        }

        public static float GetSpawnFrequency() => Raid.GetSpawnFrequency((int)timeSinceStart);
    }
}