using System.Collections.Generic;
using GameCore.Battle.Data;
using UnityEngine;

namespace BeatRoyale.Windows
{
    public class HeroesWindow : BaseLauncherWindow<HeroesWindow>
    {
        [SerializeField] private Transform slotsParent;
        [SerializeField] private HeroView heroPrefab;

        private Dictionary<string, HeroView> heroes = new();
        
        protected override void OnShowing()
        {
            base.OnShowing();
            foreach (var hero in Cards.ByName)
            {
                var heroView = Instantiate(heroPrefab, slotsParent);
                heroView.Init(hero.Key, hero.Value);
                heroes.Add(hero.Key, heroView);
            }

            heroes[PlayerData.Config.SelectedHero].SetSelected(true);
        }

        protected override void OnHiding()
        {
            base.OnHiding();
            foreach (var hero in heroes)
            {
               Destroy(hero.Value.gameObject);
            }
            heroes.Clear();
        }
    }
}