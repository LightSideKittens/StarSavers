using LSCore;
using LSCore.Extensions.Unity;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace BeatHeroes.Windows
{
    internal class MainWindow : BaseWindow<MainWindow>
    {
        public static int CurrentShowingWindowIndex { get; set; }
        public static int CurrentShowedWindowIndex { get; set; }

        [SerializeField] private Button burgerButton;
        [SerializeField] private Button heroButton;
        [SerializeField] private Button battleButton;
        [SerializeField] private Button clanButton;
        [SerializeField] private Button questsButton;
        [SerializeField] private Button shopButton;

        protected override void Init()
        {
            base.Init();
            burgerButton.AddListener(BurgerPanel.Show);
            heroButton.AddListener(HeroesGalleryWindow.Show);
            clanButton.AddListener(ClanWindow.Show);
            questsButton.AddListener(QuestsWindow.Show);
            shopButton.AddListener(ShopWindow.Show);
            battleButton.AddListener(LoadBattle);
        }

        private static void LoadBattle() => SceneManager.LoadScene(1);
    }
}