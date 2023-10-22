using System;
using DG.Tweening;
using UnityEngine.Scripting;
using Health = GameCore.Battle.Data.Components.HealthComponent;

namespace GameCore.Battle.Data.Components
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
                    var target = findTargetComponent.target;
                    TryApplyDamage(target);
                    if(transform.TryGet<Health>(out var health)) health.Kill();
                });
        }
    }
}