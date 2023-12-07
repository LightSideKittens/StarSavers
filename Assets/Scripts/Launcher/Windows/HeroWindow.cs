using LSCore;
using UnityEngine;

namespace BeatHeroes.Windows
{
    public class HeroWindow : BaseLauncherWindow<HeroWindow>
    {
        protected override int Internal_Index => 1;
        [SerializeField] private LSButton homeButton;

        protected override void Init()
        {
            base.Init();
            homeButton.Clicked += OnHome;
        }

        private void OnHome()
        {
            Hide();
            MainWindow.Show();
        }
    }
}