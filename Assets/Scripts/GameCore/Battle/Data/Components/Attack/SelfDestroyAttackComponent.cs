using DG.Tweening;
using UnityEngine;

namespace GameCore.Battle.Data.Components
{
    internal class SelfDestroyAttackComponent : AttackComponent
    {
        protected override Tween AttackAnimation(Vector2 targetPosition)
        {
            return base.AttackAnimation(targetPosition).SetLoops(1, LoopType.Yoyo)
                .OnComplete(() => Unit.ByTransform[transform].healthComponent.Kill());
        }
    }
}