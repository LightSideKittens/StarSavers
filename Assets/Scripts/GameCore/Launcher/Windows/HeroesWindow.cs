using System.Collections.Generic;
using GameCore.Battle.Data;
using UnityEngine;
using UnityEngine.UI;

namespace BeatRoyale.Windows
{
    public class HeroesWindow : BaseLauncherWindow<HeroesWindow>
    {
        protected override int Internal_Index => 1;
        
        [SerializeField] private Transform slotsParent;
        [SerializeField] private Toggle heroPrefab;

        private Dictionary<int, HeroView> heroes = new();
        
        protected override void Init()
        {
            base.Init();
            foreach (var pair in Cards.ByName)
            {
                var hero = Instantiate(heroPrefab, slotsParent);
                var heroView = new HeroView();
                heroView.Init(hero, pair.Key, pair.Value);
                heroes.Add(pair.Key, heroView);
            }

            heroes[PlayerData.Config.SelectedHero].SetSelected(true);
        }
    }
}