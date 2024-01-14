using UnityEngine;

namespace Battle.Data.Components
{
    internal class HeroAttackComponent : AttackComponent
    {
        [SerializeReference] private AttackComponent autoAttack;
        [SerializeReference] private AttackComponent mainAttack;
        
        
        protected override void OnInit()
        {
            
        }
    }
}