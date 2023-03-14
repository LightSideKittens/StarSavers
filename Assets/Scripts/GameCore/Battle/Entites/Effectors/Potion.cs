using System;
using Battle.Data;
using Battle.Data.GameProperty;

namespace GameCore.Battle.Data
{
    [Serializable]
    internal class Potion : BaseEffector
    {
        private float duration;
        private float damage;
        protected override bool NeedFindOpponent => false;

        protected override void OnInit()
        {
            var properties = EntitiesProperties.ByName[name];
            duration = properties[nameof(HealthGP)].Value;
            damage = properties[nameof(DamageGP)].Value / 100f;
        }
        
        protected override void OnApply()
        {
        }
    }
}