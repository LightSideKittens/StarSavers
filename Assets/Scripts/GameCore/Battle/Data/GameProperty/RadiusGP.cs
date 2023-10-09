using System;

namespace Battle.Data.GameProperty
{
    [Serializable]
    public class RadiusGP : FloatAndPercent
    {
#if UNITY_EDITOR
        protected override string IconName { get; }
#endif
    }
}