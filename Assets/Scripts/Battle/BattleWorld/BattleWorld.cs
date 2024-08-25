using Animatable;
using Battle;
using Battle.Windows;
using MultiWars.Interfaces;
using DG.Tweening;
using Battle.Data;
using LSCore.Extensions.Unity;
using UnityEngine;

namespace LSCore.BattleModule
{
    public partial class BattleWorld : ServiceManager<BattleWorld>
    {
        [SerializeField] private RaidSetupper raids;
        public static RaidConfig Raid => Instance.raids.Current;
        private static int currentWave;
        private static RaidConfig.Wave wave;
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
            
            raids.Setup().OnComplete(() =>
            {
                BattleWindow.Show();
                Unit.Releasedd += OnUnitReleasedd;
                PlayerWorld.Begin();
                OpponentWorld.Begin();
                currentWave = 0;
                StartNextWave();
            });
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            Raid.Dispose();
            DOTween.KillAll();
            Unit.Releasedd -= OnUnitReleasedd;
        }

        private static void StartNextWave()
        {
            wave = Raid.GetWave();
            isLastWave = !Raid.MoveNextWave();
            foreach (var action in wave.actions)
            {
                action.OnStart();
            }
        }
        
        private static void OnWaveCompleted()
        {
            Debug.Log("Wave Completed");
            foreach (var action in wave.actions)
            {
                action.OnComplete();
            }

            if (isLastWave)
            {
                Win();
            }
            else
            {
                StartNextWave();
            }
        }

        private static void Win()
        {
            MatchResultWindow.Show(true);
        }
        
        private static void OnUnitReleasedd(Unit unit)
        {
            
        }
    }
}