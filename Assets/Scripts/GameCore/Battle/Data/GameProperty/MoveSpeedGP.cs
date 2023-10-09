using System;

namespace Battle.Data.GameProperty
{
    [Serializable]
    public class MoveSpeedGP : FloatAndPercent
    {
#if UNITY_EDITOR
        protected override string IconName { get; }
#endif
    }
}