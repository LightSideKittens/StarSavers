using System;

namespace Battle.Data.GameProperty
{
    [Serializable]
    public class RicochetGP : FloatAndPercent
    {
#if UNITY_EDITOR
        protected override string IconName => "ricochet-icon";
#endif
    }
}