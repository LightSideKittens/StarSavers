using System;

namespace Battle.Data.GameProperty
{
    public class GameProperties
    {
        
    }
    
    [Serializable]
    public class DamageGP : BaseGameProperty { }
    
    [Serializable]
    public class HealthGP : BaseGameProperty { }
    
    [Serializable]
    public class MoveSpeedGP : BaseGameProperty { }
    
    [Serializable]
    public class AttackSpeedGP : BaseGameProperty { }
    
    [Serializable]
    public class RadiusGP : BaseGameProperty { }
}