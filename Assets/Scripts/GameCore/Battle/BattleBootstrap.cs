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
        [SerializeField] private MeshRenderer spawnArea; 
        public static MeshRenderer SpawnArea { get; private set; }

        protected override void Awake()
        {
            base.Awake();
            SpawnArea = spawnArea;
        }

        private void Start()
        {
            units.Init();
            cards.Init();
            DeckWindow.Show();
        }
    }
}