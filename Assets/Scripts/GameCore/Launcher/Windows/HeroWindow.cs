using LSCore.Extensions.Unity;
using UnityEngine;
using UnityEngine.UI;

namespace BeatRoyale.Windows
{
    public class HeroWindow : BaseLauncherWindow<HeroWindow>
    {
        protected override int Internal_Index => 1;
        [SerializeField] private Button backButton;
        [SerializeField] private Button homeButton;
        protected override Button BackButton => backButton;

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