using LSCore;
using UnityEngine;

namespace BeatHeroes.Windows
{
    public class BurgerPanel : BaseWindow<BurgerPanel>
    {
        [SerializeField] private RectTransform back;
        [SerializeReference] private Tab.BaseData buttonsTab;
        [SerializeReference] private Tab.BaseData[] tabs;
        
        private Tab.Controller tabController;

        protected override void Init()
        {
            base.Init();
            tabController = new Tab.Controller(back);
            tabController.OnOpen(OnTabOpen);
            tabController.Add(buttonsTab);
            tabController.Register(tabs);
        }

        private void OnTabOpen(Tab tab)
        {
            if (tab == buttonsTab.tabPrefab)
            {
                BackButton.Clicked = Hide;
            }
            else
            {
                BackButton.Clicked = OpenButtonsTab;
            }
        }

        private void OpenButtonsTab()
        {
            tabController.Open(buttonsTab);
        }
    }
}