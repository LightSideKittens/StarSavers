using UnityEngine;
using UnityEngine.UI;

namespace BeatHeroes.Windows
{
    public class HeroesGalleryWindow : BaseLauncherWindow<HeroesGalleryWindow>
    {
        protected override int Internal_Index => 0;

        [SerializeField] private Button backButton;
        protected override Button BackButton => backButton;

        protected override void OnBackButton()
        {
            base.OnBackButton();
            MainWindow.Show();
        }
    }
}