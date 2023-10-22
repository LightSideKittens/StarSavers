using System;
using DG.Tweening;
using UnityEngine;
using Health = GameCore.Battle.Data.Components.HealthComponent;

namespace GameCore.Battle.Data.Components
{
    [Serializable]
    internal class SelfDestroyAttackComponent : AttackComponent
    {
        protected override Tween AttackAnimation()
        {
            attackLoopEmiter.Kill();
            return transform.DOMove(lastHitPoint, duration)
                .OnComplete(() =>
                {
                    var target = findTargetComponent.target;
                    target.Get<Health>().TakeDamage(Damage);
                    transform.Get<Health>().Kill();
                });
        }
    }
}