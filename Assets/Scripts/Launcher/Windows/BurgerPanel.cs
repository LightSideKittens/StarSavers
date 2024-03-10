using LSCore;
using UnityEngine;

namespace MultiWars.Windows
{
    public class BurgerPanel : BaseWindow<BurgerPanel>
    {
        [SerializeField] private RectTransform back;
        [SerializeReference] private Tab.BaseData buttonsTab;
        [SerializeReference] private Tab.BaseData[] tabs;
        [SerializeField] private LSButton shopButton;
        private Tab.Controller tabController;
        protected override bool NeedHideAllPrevious => true;

        protected override void Init()
        {
            base.Init();
            tabController = new Tab.Controller(back);
            tabController.OnOpen(OnTabOpen);
            tabController.Add(buttonsTab);
            tabController.Register(tabs);
            shopButton.Clicked += ShopWindow.Show;
        }

        private void OnTabOpen(Tab tab)
        {
            if (tab == buttonsTab.tabPrefab)
            {
                BackButton.Clicked = OnBackButton;
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