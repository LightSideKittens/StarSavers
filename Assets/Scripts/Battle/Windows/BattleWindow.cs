using LSCore;
using UnityEngine;

namespace Battle.Windows
{
    public class BattleWindow : BaseWindow<BattleWindow>
    {
        [SerializeField] private Joystick joystick;

        public static Joystick Joystick => Instance.joystick;

        protected override void OnBackButton()
        {
            base.OnBackButton();
            MatchResultWindow.Show(false);
            
        }
    }
}