using LSCore;
using UnityEngine;

namespace Battle.Windows
{
    public class BattleWindow : BaseWindow<BattleWindow>
    {
        [SerializeField] private FloatingJoystick joystick;

        public static FloatingJoystick Joystick => Instance.joystick;
    }
}