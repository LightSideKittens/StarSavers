using LSCore;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BeatHeroes.Windows
{
    internal class MainWindow : BaseWindow<MainWindow>
    {
        public static int CurrentShowingWindowIndex { get; set; }
        public static int CurrentShowedWindowIndex { get; set; }

        [SerializeField] private LSButton burgerButton;
        [SerializeField] private LSButton heroButton;
        [SerializeField] private LSButton battleButton;
        [SerializeField] private LSButton clanButton;
        [SerializeField] private LSButton questsButton;
        [SerializeField] private LSButton shopButton;

        protected override void Init()
        {
            base.Init();
            burgerButton.Clicked += BurgerPanel.Show;
            heroButton.Clicked += HeroesGalleryWindow.Show;
            clanButton.Clicked += ClanWindow.Show;
            questsButton.Clicked += QuestsWindow.Show;
            shopButton.Clicked += ShopWindow.Show;
            battleButton.Clicked += LoadBattle;
        }

        private static void LoadBattle() => SceneManager.LoadScene(1);
    }
}