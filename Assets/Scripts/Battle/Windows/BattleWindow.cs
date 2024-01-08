using DG.Tweening;
using LSCore;
using LSCore.AnimationsModule;
using UnityEngine;

namespace Battle.Windows
{
    public class BattleWindow : BaseWindow<BattleWindow>
    {
        [SerializeField] private Joystick joystick;
        [SerializeField] private LSText statusText;
        [SerializeField] private LSText splashText;
        [SerializeField] private AnimSequencer splash;
        
        public static Joystick Joystick => Instance.joystick;
        public static LSText StatusText => Instance.statusText;

        protected override void OnBackButton()
        {
            base.OnBackButton();
            MatchResultWindow.Show(false);
        }

        public static void SplashText(string text)
        {
            Instance.splashText.text = text;
            Instance.splash.Animate();
        }
    }
}