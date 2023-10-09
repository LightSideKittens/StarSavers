using LSCore;
using LSCore.Extensions.Unity;
using UnityEngine;
using UnityEngine.UI;

namespace BeatRoyale.Windows
{
    internal class MainWindow : BaseWindow<MainWindow>
    {
        public static int CurrentShowingWindowIndex { get; set; }
        public static int CurrentShowedWindowIndex { get; set; }

        [SerializeField] private Button burgerButton;
        [SerializeField] private Button heroButton;

        protected override void Init()
        {
            base.Init();
            burgerButton.AddListener(BurgerPanel.Show);
            heroButton.AddListener(Hide);
            heroButton.AddListener(HeroesGalleryWindow.Show);
        }
    }
}