using Battle.Windows;
using Battle.Data;
using UnityEngine;

namespace Battle
{
    public class PlayerWorld : BasePlayerWorld<PlayerWorld>
    {
        [SerializeField] private UnitsById heroes;
        [SerializeField] private Vector3 cameraOffset;
        public static Transform HeroTransform { get; private set; }
        private Unit hero;
        
        protected override void OnBegin()
        {
            UserId = "Player";
            var prefab = heroes.ById[PlayerData.Config.SelectedHero];
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

        protected override void OnStop()
        {
            Unit.DestroyPool(hero.Id);
        }
    }
}