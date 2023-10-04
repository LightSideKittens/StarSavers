using System.Collections.Generic;
using GameCore.Battle.Data;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace BeatRoyale.Windows
{
    public class HeroesWindow : BaseLauncherWindow<HeroesWindow>
    {
        [SerializeField] private Transform slotsParent;
        [SerializeField] private Toggle heroPrefab;

        private Dictionary<string, HeroView> heroes = new();
        
        protected override void OnShowing()
        {
            base.OnShowing();
            foreach (var pair in Cards.ByName)
            {
                var hero = Instantiate(heroPrefab, slotsParent);
                var heroView = new HeroView();
                heroView.Init(hero, pair.Key, pair.Value);
                heroes.Add(pair.Key, heroView);
            }

            heroes[PlayerData.Config.SelectedHero].SetSelected(true);
        }

        protected override void OnHiding()
        {
            base.OnHiding();
            ClearHeroes();
        }

        protected override void DeInit()
        {
            base.DeInit();
            ClearHeroes();
        }

        private void ClearHeroes()
        {
            foreach (var hero in heroes)
            {
                Destroy(hero.Value.Toggle.gameObject);
            }
            heroes.Clear();        }
    }
}