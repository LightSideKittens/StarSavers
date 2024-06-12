using LSCore;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace MultiWars.Windows
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
        
        public RawImage emojiImage;

        void Start()
        {
            if (EmojiRenderer.TryRenderEmoji("❤", 128, out var texture))
            {
                emojiImage.texture = texture;
            }
        }

        protected override void OnShowing()
        {
            CurrentShowedWindowIndex = 0;
            CurrentShowingWindowIndex = 0;
        }

        private static void LoadBattle() => SceneManager.LoadScene(1);
    }
}