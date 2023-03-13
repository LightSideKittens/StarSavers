using Battle.Windows;
using Core.SingleService;
using GameCore.Battle.Data;
using UnityEngine;

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
            units.Init();
            cards.Init();
            effectors.Init();
            DeckWindow.Show();
        }
    }
}