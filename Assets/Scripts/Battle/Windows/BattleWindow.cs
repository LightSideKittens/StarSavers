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
        [SerializeField] private ProgressJoystick ultimateJoystick;
        [SerializeField] private ProgressJoystick skillJoystick;
        [SerializeField] private HealthBar bossHealth;
        
        public static Joystick Joystick => Instance.joystick;
        public static ProgressJoystick UltimateJoystick => Instance.ultimateJoystick;
        public static ProgressJoystick SkillJoystick => Instance.skillJoystick;
        public static LSText StatusText => Instance.statusText;
        public static HealthBar BossHealth => Instance.bossHealth;

        public static bool IsBossMode
        {
            set => BossHealth.gameObject.SetActive(value);
        }

        public static void SplashText(string text)
        {
            Instance.splashText.text = text;
            Instance.splash.Animate();
        }
    }
}