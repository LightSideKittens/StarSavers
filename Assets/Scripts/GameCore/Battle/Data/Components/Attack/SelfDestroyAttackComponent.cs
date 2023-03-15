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
            return base.AttackAnimation().SetLoops(1, LoopType.Yoyo)
                .OnComplete(() => transform.Get<Health>().Kill());
        }
    }
}