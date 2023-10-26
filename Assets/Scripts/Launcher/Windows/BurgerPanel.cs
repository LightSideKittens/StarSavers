using LSCore;
using UnityEngine;

namespace BeatHeroes.Windows
{
    public class BurgerPanel : BaseWindow<BurgerPanel>
    {
        [SerializeField] private RectTransform back;
        [SerializeField] private Tab.Data buttonsTab;
        [SerializeField] private Tab.Data[] tabs;
        
        private Tab.Controller tabController;

        protected override void Init()
        {
            base.Init();
            tabController = new Tab.Controller(back);
            tabController.OnOpen(OnTabOpen);
            tabController.Add(buttonsTab);

            for (int i = 0; i < tabs.Length; i++)
            {
                tabController.Register(tabs[i]);
            }
        }

        private void OnTabOpen(Tab tab)
        {
            if (tab == buttonsTab.tabPrefab)
            {
                BackButton.OnClick(Hide);
            }
            else
            {
                BackButton.OnClick(OpenButtonsTab);
            }
        }

        private void OpenButtonsTab()
        {
            tabController.Open(buttonsTab);
        }
    }
}