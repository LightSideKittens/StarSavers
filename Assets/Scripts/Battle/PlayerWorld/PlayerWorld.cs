using Battle.Windows;
using Battle.Data;
using LSCore.BattleModule;
using UnityEngine;

namespace Battle
{
    public class PlayerWorld : BasePlayerWorld<PlayerWorld>
    {
        [SerializeField] private UnitsById heroes;
        [SerializeField] private Vector3 cameraOffset;
        public static Transform HeroTransform { get; private set; }
        private Unit hero;

        public override string UserId => BattlePlayerData.UserId;
        public override string TeamId => BattlePlayerData.TeamId;

        protected override void OnBegin()
        {
            var prefab = heroes.ByKey[PlayerData.Config.SelectedHero];
            var pool = CreatePool(prefab);
            pool.Released += OnHeroDied;
            hero = pool.Get();
            HeroTransform = hero.transform;
            CameraMover.Init(Camera.main, HeroTransform, cameraOffset);
        }

        private void OnHeroDied(Unit unit)
        {
            MatchResultWindow.Show(false);
        }
    }
}