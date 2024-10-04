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
        [SerializeField] protected FindTargetFactory findTargetFactory;
        [SerializeReference] protected List<BaseAttack> attacks;
        
        protected FindTargetComp findTargetComp;
        public float cooldown = 0.5f;
        
        protected override void Init()
        {
            findTargetComp = findTargetFactory.Create();
            findTargetComp.Init(transform);
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