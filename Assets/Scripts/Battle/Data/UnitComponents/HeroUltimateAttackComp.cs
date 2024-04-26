using System;
using Battle.Windows;
using LSCore.BattleModule;
using UnityEngine.Scripting;

namespace Battle.Data.UnitComponents
{
    [Preserve, Serializable]
    public class HeroUltimateAttackComp : HeroSkillAttackComp
    {
        protected override ProgressJoystick Joystick => BattleWindow.UltimateJoystick;
    }
}