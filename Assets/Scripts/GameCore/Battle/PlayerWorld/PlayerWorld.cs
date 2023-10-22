using Battle.Windows;
using GameCore.Battle;
using GameCore.Battle.Data;
using UnityEngine;

namespace Battle
{
    public class PlayerWorld : BasePlayerWorld<PlayerWorld>
    { 
        [SerializeField] private Heroes heroes;
        [SerializeField] private Vector3 cameraOffset;
        private Unit hero;
        
        protected override void OnBegin()
        {
            UserId = "Player";
            heroes.Init();
            hero = Spawn(Heroes.ByName[PlayerData.Config.SelectedHero]);
            hero.Enable();
            hero.Destroyed += OnHeroDied;
            CameraMover.Init(Camera.main, hero.transform, cameraOffset);
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