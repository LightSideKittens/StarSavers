using System;

namespace LSCore.LevelSystem
{
    [Serializable]
    public class AreaDamageGP : FloatAndPercent
    {
#if UNITY_EDITOR
        protected override string IconName => "area-damage-icon";
#endif
    }
}