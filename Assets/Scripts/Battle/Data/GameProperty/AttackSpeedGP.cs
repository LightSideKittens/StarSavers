using System;

namespace Battle.Data.GameProperty
{
    [Serializable]
    public class AttackSpeedGP : FloatAndPercent
    {
#if UNITY_EDITOR
        protected override Prop DrawFields()
        {
            prop.DrawSlider(ValueKey, 0.1f, 2f);
            prop.DrawSlider(PercentKey, 0, 100);
            return prop;
        }
        protected override string IconName => "attack-speed-icon";
#endif
    }
}