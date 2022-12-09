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
            [SerializeField] private Tab workshopTab;
            [SerializeField] private Tab matchTab;
            [SerializeField] private Tab ratingTab;
            [SerializeField] private Tab gameEventsTab;

            [ColoredField, SerializeField] private TabAnimationData tabsAnimationData;

            public void Init()
            {
                Tab.animationData = tabsAnimationData;
                shopTab.Init<ShopWindow>();
                workshopTab.Init<WorkshopWindow>();
                matchTab.Init<MatchWindow>();
                ratingTab.Init<RatingWindow>();
                gameEventsTab.Init<GameEventsWindow>();
            }
        }
    }
}