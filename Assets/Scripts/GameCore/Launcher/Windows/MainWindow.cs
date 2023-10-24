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

        [SerializeField] private LSButton burgerButton;
        [SerializeField] private LSButton heroButton;
        [SerializeField] private LSButton battleButton;
        [SerializeField] private LSButton clanButton;
        [SerializeField] private LSButton questsButton;
        [SerializeField] private LSButton shopButton;

        protected override void Init()
        {
            base.Init();
            burgerButton.Listen(BurgerPanel.Show);
            heroButton.Listen(HeroesGalleryWindow.Show);
            clanButton.Listen(ClanWindow.Show);
            questsButton.Listen(QuestsWindow.Show);
            shopButton.Listen(ShopWindow.Show);
            battleButton.Listen(LoadBattle);
        }

        private static void LoadBattle() => SceneManager.LoadScene(1);
    }
}