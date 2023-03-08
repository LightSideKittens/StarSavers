using System;
using Common.SingleServices;
using DG.Tweening;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GameCore.Battle.Data.Components
{
    [Serializable]
    internal class CannonAttackComponent : AttackComponent
    {
        [SerializeField] private GameObject bulletPrefab;
        [SerializeField] private ParticleSystem deathFx;
        
        protected override Tween AttackAnimation(Vector2 targetPosition)
        {
            var bullet = Object.Instantiate(bulletPrefab, transform.position, Quaternion.identity);
            var target = findTargetComponent.target;
            return bullet.transform.DOMove(target.position, duration).SetEase(Ease.InExpo).OnComplete(() =>
            {
                new CountDownTimer(0.35f, true).Stopped += () => Object.Destroy(bullet);
                HealthComponent.ByTransform[target].TakeDamage(damage);
                var pos = target.position;
                AnimText.Create($"{damage}", pos, fromWorldSpace: true);
                Object.Instantiate(deathFx, findTargetComponent.target.position, Quaternion.identity);
            });
        }
    }
}