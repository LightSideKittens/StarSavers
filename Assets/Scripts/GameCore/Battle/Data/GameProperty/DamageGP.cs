using System;

namespace Battle.Data.GameProperty
{
    [Serializable]
    public class DamageGP : FloatAndPercent
    {
#if UNITY_EDITOR
        protected override string IconName => "attack-icon";
#endif
    }
}