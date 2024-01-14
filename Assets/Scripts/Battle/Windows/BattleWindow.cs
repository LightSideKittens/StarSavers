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
        [SerializeField] private LSButton mainButton;
        [SerializeField] private LSButton ultimateButton;
        [SerializeField] private LSButton skillButton;
        
        public static Joystick Joystick => Instance.joystick;
        public static LSText StatusText => Instance.statusText;
        public static LSButton MainButton => Instance.mainButton;
        public static LSButton UltimateButton => Instance.ultimateButton;
        public static LSButton SkillButton => Instance.skillButton;

        protected override void OnBackButton() => MatchResultWindow.Show(false);

        public static void SplashText(string text)
        {
            Instance.splashText.text = text;
            Instance.splash.Animate();
        }
    }
}