using LSCore;
using UnityEngine;

namespace Battle.Windows
{
    public class BattleWindow : BaseWindow<BattleWindow>
    {
        [SerializeField] private Joystick joystick;

        public static Joystick Joystick => Instance.joystick;
    }
}