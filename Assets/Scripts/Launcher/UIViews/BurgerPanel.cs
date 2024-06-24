using DG.Tweening;
using LSCore;
using UnityEngine;

namespace MultiWars.Windows
{
    public class BurgerPanel : CanvasView
    {
        [SerializeField] private UIView buttonsPrefab;
        private UIView buttonsView;
        [SerializeField] private AnimOnChildAdded content;
        [SerializeField] private RectTransform back;
        [SerializeReference] private Tab.BaseData buttonsTab;
        [SerializeReference] private Tab.BaseData[] tabs;
        private Tab.Controller tabController;

        protected override bool UseDefaultAnimation => false;
        
        

        protected override void Init()
        {
            base.Init();
            
            /*tabController = new Tab.Controller(back);
            tabController.OnOpen(OnTabOpen);
            tabController.Add(buttonsTab);
            tabController.Register(tabs);*/
        }

        protected override void OnShowing()
        {
            base.OnShowing();
            buttonsView ??= Instantiate(buttonsPrefab, content.transform);
            buttonsView.Show();
            content.Show();
        }

        protected override Tween HideAnim => content.Hide();

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