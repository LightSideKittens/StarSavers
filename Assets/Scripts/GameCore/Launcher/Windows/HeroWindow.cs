using LSCore.Extensions.Unity;
using UnityEngine;
using UnityEngine.UI;

namespace BeatHeroes.Windows
{
    public class HeroWindow : BaseLauncherWindow<HeroWindow>
    {
        protected override int Internal_Index => 1;
        [SerializeField] private Button homeButton;

        protected override void Init()
        {
            base.Init();
            homeButton.AddListener(OnHome);
        }

        private void OnHome()
        {
            Hide();
            MainWindow.Show();
        }
    }
}