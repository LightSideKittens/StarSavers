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

namespace Battle
{
    public class BattleBootstrap : ServiceManager
    {
        [SerializeField] private Units units;
        [SerializeField] private Cards cards;
        [SerializeField] private Effectors effectors;
        [SerializeField] private MeshRenderer spawnArea;
        [SerializeField] private BoxCollider2D arenaBox;
        public static MeshRenderer SpawnArea { get; private set; }
        public static BoxCollider2D ArenaBox { get; private set; }

        protected override void Awake()
        {
            base.Awake();
            SpawnArea = spawnArea;
            ArenaBox = arenaBox;
        }

        private void Start()
        {
            if (MatchPlayersData.Count == 0)
            {
                var loader = Loader.Create();
                Action onSuccess = Init;
                onSuccess += loader.Destroy;
                Leaderboards.GetUserId(userId =>
                {
                    MatchPlayersData.Add(userId, onSuccess);
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
            OpponentWorld.userId = MatchPlayersData.OpponentUserId;
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