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

        protected override void OnShowing()
        {
            MainWindow.Hide();
        }

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

        protected override void OnBackButton()
        {
            base.OnBackButton();
            MainWindow.Show();
        }

        private void OnTabOpen(Tab tab) 
        {
            scroller.content = tab.GetComponent<RectTransform>();
            var buttonTransform = tabController.CurrentData.Clickable.Transform;
            selectedTabPointer.position = buttonTransform.position;
        }
    }
}