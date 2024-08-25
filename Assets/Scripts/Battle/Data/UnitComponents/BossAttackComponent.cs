using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using LSCore;
using LSCore.Async;
using LSCore.BattleModule;
using LSCore.Extensions;
using UnityEngine;

namespace Battle.Data.UnitComponents
{
    [Serializable]
    public class BossAttackComponent : BaseComp
    {
        [SerializeField] protected FindTargetComp findTargetComp;
        [SerializeReference] protected List<BaseAttack> attacks;
        public float cooldown = 0.5f;
        
        protected override void Init()
        {
            for (int i = 0; i < attacks.Count; i++)
            {
                attacks[i].Init(transform, findTargetComp);
            }
            
            Attack();
        }

        private void Attack()
        {
            var attack = attacks.Where(x => x.CanAttack).RandomElement();
            if (attack != null)
            {
                DOTween.Sequence()
                    .Append(attack.Attack())
                    .AppendInterval(cooldown)
                    .OnComplete(Attack);
            }
            else
            {
                Wait.Frames(1, Attack);
            }
        }
    }
}