using System;
using GameCore.Attributes;
using UnityEngine;

namespace BeatRoyale.Windows
{
    internal partial class ControlPanel
    {
        [Serializable]
        private struct Tabs
        {
            [SerializeField] private Tab shopTab;
            [SerializeField] private Tab cardGalleryTab;
            [SerializeField] private Tab matchTab;
            [SerializeField] private Tab clanTab;
            [SerializeField] private Tab gameEventsTab;

            [ColoredField, SerializeField] private TabAnimationData tabsAnimationData;

            public void Init()
            {
                Tab.animationData = tabsAnimationData;
                shopTab.Init<ShopWindow>();
                cardGalleryTab.Init<HeroesWindow>();
                matchTab.Init<MatchWindow>();
                clanTab.Init<ClanWindow>();
                gameEventsTab.Init<GameEventsWindow>();
                matchTab.SetActive();
            }
        }
    }
}