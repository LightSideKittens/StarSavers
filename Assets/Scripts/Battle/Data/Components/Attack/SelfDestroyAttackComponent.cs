using System;
using DG.Tweening;
using UnityEngine.Scripting;
using Health = Battle.Data.Components.HealthComponent;

namespace Battle.Data.Components
{
    [Preserve, Serializable]
    internal class SelfDestroyAttackComponent : AttackComponent
    {
        protected override Tween AttackAnimation()
        {
            attackLoopEmiter.Kill();
            return transform.DOMove(lastHitPoint, duration)
                .OnComplete(() =>
                {
                    TryApplyDamage();
                    if(transform.TryGet<Health>(out var health)) health.Kill();
                });
        }
    }
}