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
        [SerializeField] private Joystick ultimateJoystick;
        [SerializeField] private Joystick skillJoystick;
        
        public static Joystick Joystick => Instance.joystick;
        public static Joystick UltimateJoystick => Instance.ultimateJoystick;
        public static Joystick SkillJoystick => Instance.skillJoystick;
        public static LSText StatusText => Instance.statusText;

        protected override void OnBackButton() => MatchResultWindow.Show(false);

        public static void SplashText(string text)
        {
            Instance.splashText.text = text;
            Instance.splash.Animate();
        }
    }
}