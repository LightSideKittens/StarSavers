using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Scripting;
using static UnityEngine.Object;

namespace Battle.Data.Components
{
    [Preserve, Serializable]
    internal class ShootAttackComponent : AttackComponent
    {
        [SerializeField] private GameObject bulletPrefab;
        
        protected override Tween AttackAnimation()
        {
            var bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
            return bullet.transform.DOMove(target.position, duration).OnComplete(TryApplyDamage);
        }
    }
}