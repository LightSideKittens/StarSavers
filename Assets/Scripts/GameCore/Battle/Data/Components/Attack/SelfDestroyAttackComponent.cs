using System;
using DG.Tweening;

namespace GameCore.Battle.Data.Components
{
    [Serializable]
    internal class SelfDestroyAttackComponent : AttackComponent
    {
        protected override Tween AttackAnimation()
        {
            return base.AttackAnimation().SetLoops(1, LoopType.Yoyo)
                .OnComplete(() => HealthComponent.ByTransform[transform].Kill());
        }
    }
}