using System;

namespace Battle.Data.GameProperty
{
    [Serializable]
    public class AreaDamageGP : FloatAndPercent
    {
#if UNITY_EDITOR
        protected override string IconName { get; }
#endif
    }
}