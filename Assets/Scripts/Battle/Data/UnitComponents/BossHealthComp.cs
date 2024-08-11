using System;
using Animatable;
using Battle.Windows;
using LSCore.BattleModule;

namespace Battle.Data.UnitComponents
{
    [Serializable]
    public class BossHealthComp : BaseHealthComp
    {
        protected override void OnDamageTaken(float damage)
        {
            base.OnDamageTaken(damage);
            BattleWindow.BossHealth.value -= damage;
            AnimText.Create($"{(int)damage}", transform.position);
        }
    }
}