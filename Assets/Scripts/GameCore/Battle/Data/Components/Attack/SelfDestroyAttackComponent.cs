using System;
using DG.Tweening;
using Health = GameCore.Battle.Data.Components.HealthComponent;

namespace GameCore.Battle.Data.Components
{
    [Serializable]
    internal class SelfDestroyAttackComponent : AttackComponent
    {
        protected override Tween AttackAnimation()
        {
            var anim = base.AttackAnimation();
            anim.onComplete += () => transform.Get<Health>().Kill();
            return anim;
        }
    }
}