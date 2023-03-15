using Battle.Windows;
using BeatRoyale;
using Core.SingleService;
using GameCore.Battle.Data;
using UnityEngine;
using static GameCore.Battle.Data.Cannon;
using static GameCore.Battle.Data.Tower;

namespace Battle
{
    public class BattleBootstrap : ServiceManager
    {
        [SerializeField] private string opponentUserId;
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
                MatchPlayersData.Add(opponentUserId, Init);
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
            OpponentWorld.userId = opponentUserId;
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