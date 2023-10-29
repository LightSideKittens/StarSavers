using Battle.Windows;
using Battle.Data;
using UnityEngine;

namespace Battle
{
    public class PlayerWorld : BasePlayerWorld<PlayerWorld>
    {
        //[SerializeField] private Heroes heroes;
        [SerializeField] private Vector3 cameraOffset;
        public static Transform HeroTransform { get; private set; }
        private Unit hero;
        
        protected override void OnBegin()
        {
            UserId = "Player";
            //heroes.Init();
            //hero = Spawn(Heroes.ByKey[PlayerData.Config.SelectedHero]);
            hero.Enable();
            hero.Destroyed += OnHeroDied;
            HeroTransform = hero.transform;
            CameraMover.Init(Camera.main, HeroTransform, cameraOffset);
        }

        protected override void OnStop()
        {
            hero.Destroyed -= OnHeroDied;
            hero.Destroy();
        }

        private void OnHeroDied()
        {
            MatchResultWindow.Show(false);
        }
    }
}