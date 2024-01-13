using LSCore;
using LSCore.Async;
using UnityEngine;
using UnityEngine.UI;

namespace BeatHeroes.Windows
{
    public class ShopWindow : BaseWindow<ShopWindow>
    {
        [SerializeField] private ScrollRect scroller;
        [SerializeField] private RectTransform selectedTabPointer;
        
        [SerializeField] private RectTransform back;
        [SerializeReference] private Tab.BaseData[] tabs;
        private Tab.Controller tabController;
        protected override bool NeedHidePrevious => false;

        protected override void Init()
        {
            base.Init();
            tabController = new Tab.Controller(back);
            tabController.OnOpen(OnTabOpen);
            tabController.Register(tabs);
        }

        private void OnEnable()
        {
            Wait.Frames(1, () => tabController?.Open(tabs[0]));
        }

        private void OnTabOpen(Tab tab)
        {
            var clickable = tabController.CurrentData.Clickable;
            clickable.Clicked += BurgerPanel.Show;
            scroller.content = tab.GetComponent<RectTransform>();
            var buttonTransform = clickable.Transform;
            selectedTabPointer.position = buttonTransform.position;
        }
    }
}