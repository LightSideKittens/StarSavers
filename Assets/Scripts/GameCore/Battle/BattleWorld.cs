using System;
using Battle.Windows;
using BeatRoyale;
using Common.SingleServices;
using Core.Server;
using Core.SingleService;
using GameCore.Battle.Data;
using UnityEngine;
using static GameCore.Battle.Data.Cannon;
using static GameCore.Battle.Data.Tower;
using Initializer = BeatRoyale.Interfaces.BaseInitializer<BeatRoyale.Interfaces.IInitializer>;

namespace Battle
{
    public class BattleWorld : ServiceManager
    {
        [SerializeField] private Units units;
        [SerializeField] private Cards cards;
        [SerializeField] private Effectors effectors;
        [SerializeField] private MeshRenderer spawnArea;
        [SerializeField] private MeshRenderer opponentSpawnArea;
        [SerializeField] private BoxCollider2D arenaBox;
        public static MeshRenderer SpawnArea { get; private set; }
        public static MeshRenderer OpponentSpawnArena { get; private set; }
        public static BoxCollider2D ArenaBox { get; private set; }

        protected override void Awake()
        {
            base.Awake();
            SpawnArea = spawnArea;
            OpponentSpawnArena = opponentSpawnArea;
            ArenaBox = arenaBox;
        }

        private void Start()
        {
            Initializer.Initialize(OnInitialize);
        }

        private void OnInitialize()
        {
            if (MatchData.Count == 0)
            {
                var loader = Loader.Create();
                Action onSuccess = Init;
                onSuccess += loader.Destroy;
                Leaderboards.GetUserId(userId =>
                {
                    MatchData.Add(userId, onSuccess);
                });
            }
            else
            {
                Init();
            }
        }

        private void Init()
        {
            units.Init();
            cards.Init();
            effectors.Init();
            MatchResultWindow.Showing += Unsubscribe;
            Tower.Destroyed += OnTowerDestroyed;
            Cannon.Destroyed += OnCannonDestroyed;
            DeckWindow.Show();
            MusicController.Begin();
        }

        private void Unsubscribe()
        {
            Tower.Destroyed -= OnTowerDestroyed;
            Cannon.Destroyed -= OnCannonDestroyed;
            MatchResultWindow.Showing -= Unsubscribe;
        }

        private void OnApplicationQuit()
        {
            Unsubscribe();
        }

        private void OnTowerDestroyed(Transform _)
        {
            if (Towers.Count == 0)
            {
                MatchResultWindow.Show(false);
            }
        }

        private void OnCannonDestroyed(Transform _)
        {
            if (Cannons.Count == 0)
            {
                MatchResultWindow.Show(true);
            }
        }
    }
}