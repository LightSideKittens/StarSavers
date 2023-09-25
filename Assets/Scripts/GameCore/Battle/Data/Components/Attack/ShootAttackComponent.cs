using System;
using DG.Tweening;
using LGCore.Async;
using UnityEngine;
using static UnityEngine.Object;

namespace GameCore.Battle.Data.Components
{
    [Serializable]
    internal class ShootAttackComponent : AttackComponent
    {
        [SerializeField] private GameObject bulletPrefab;
        
        protected override Tween AttackAnimation()
        {
            var target = findTargetComponent.target;
            var bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
            return bullet.transform.DOMove(target.position, duration).OnComplete(() =>
            {
                Wait.Delay(0.35f, () => Destroy(bullet));
                target.Get<HealthComponent>().TakeDamage(damage);
            });
        }
    }
}