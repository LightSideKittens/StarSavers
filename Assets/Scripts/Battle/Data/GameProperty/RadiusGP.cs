using System;

namespace LSCore.LevelSystem
{
    [Serializable]
    public class RadiusGP : FloatAndPercent
    {
#if UNITY_EDITOR
        protected override string IconName => "radius-icon";
#endif
    }
}