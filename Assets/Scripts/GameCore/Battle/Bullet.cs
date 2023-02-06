using System;
using Battle.ConfigsSO;
using DG.Tweening;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Battle
{
    public class Bullet<T> where T : BaseBulletsData<T>, new()
    {
        private GameObject gameObject;
        private ParticleSystem fx;
        private float damage;

        public Bullet(Castle castle, ParticleSystem fx, string soundType)
        {
            this.fx = fx;
            var bullet = BaseBulletsData<T>.GetBullet(soundType);
            damage = bullet.damage;
            gameObject = Object.Instantiate(bullet.prefab, castle.transform.position, Quaternion.identity);
        }

        public void Shoot(Enemy enemy, float duration)
        {
            enemy.ComputeDamage(damage);
            
            gameObject.transform.DOMove(enemy.Position, duration).SetEase(Ease.InCubic).OnComplete(() =>
            {
                Object.Destroy(gameObject);

                if (fx != null)
                {
                    Object.Instantiate(fx, enemy.Position, Quaternion.identity);
                }
                
                enemy.TakeDamage(damage);
            });
        }
    }
}