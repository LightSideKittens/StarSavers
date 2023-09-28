using Battle.Windows;
using GameCore.Battle.Data;
using LGCore;
using UnityEngine;
using Initializer = BeatRoyale.Interfaces.BaseInitializer<BeatRoyale.Interfaces.IInitializer>;

namespace Battle
{
    public class BattleWorld : ServiceManager
    {
        [SerializeField] private Units units;
        [SerializeField] private Cards cards;
        [SerializeField] private Effectors effectors;
        public static MeshRenderer SpawnArea { get; private set; }
        public static MeshRenderer OpponentSpawnArena { get; private set; }
        public static BoxCollider2D ArenaBox { get; private set; }

        protected override void Awake()
        {
            base.Awake();
        }

        private void Start()
        {
            Initializer.Initialize(OnInitialize);
        }

        private void OnInitialize()
        {
            Init();
        }

        private void Init()
        {
            units.Init();
            cards.Init();
            effectors.Init();
            MatchResultWindow.Showing += Unsubscribe;
            DeckWindow.Show();
        }

        private void Unsubscribe()
        {
            MatchResultWindow.Showing -= Unsubscribe;
        }

        private void OnApplicationQuit()
        {
            Unsubscribe();
        }
    }
}