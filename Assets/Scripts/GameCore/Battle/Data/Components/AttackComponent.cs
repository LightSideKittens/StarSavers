using System;
using Battle.Data;
using Battle.Data.GameProperty;
using Common.SingleServices;
using DG.Tweening;
using MusicEventSystem.Configs;
using UnityEngine;

namespace GameCore.Battle.Data.Components
{
    [Serializable]
    public class AttackComponent
    {
        private FindTargetComponent findTargetComponent;
        private string attackSpeed;
        private float damage;
        private float radius;
        private GameObject gameObject;
        private Transform transform;
        private int currentIndex;
        public bool IsInRadius { get; private set; }

        public void Init(string entityName, GameObject gameObject, FindTargetComponent findTargetComponent)
        {
            this.gameObject = gameObject;
            this.findTargetComponent = findTargetComponent;
            transform = gameObject.transform;
            var props = EntitiesProperties.Config.Properties[entityName];
            radius = props[nameof(RadiusGP)].Value / 4;
            damage = props[nameof(DamageGP)].Value;
            attackSpeed = Convert.ToString((int)props[nameof(AttackSpeedGP)].Value, 2);
            MusicData.BPMReached += OnBpmReached;
        }

        public void Update()
        {
            IsInRadius = CheckInRadius(findTargetComponent.target);
        }

        public void OnDestroy()
        {
            MusicData.BPMReached -= OnBpmReached;
        }

        public bool CheckInRadius(Transform target)
        {
            if (target != null)
            {
                var distance = Vector2.Distance(target.position, transform.position);
                distance -= target.localScale.x;
                return distance < radius;
            }

            return false;
        }

        protected virtual void AttackAnimation(Vector2 targetPosition)
        {
            transform.DOMove(targetPosition, 0.1f).SetLoops(2, LoopType.Yoyo);
        }

        private void OnBpmReached()
        {
            if (IsInRadius)
            {
                var step = attackSpeed[currentIndex % attackSpeed.Length];

                if (step == '1')
                {
                    findTargetComponent.Find();
                    var target = findTargetComponent.target;
                    HealthComponent.ByTransform[target].TakeDamage(damage);
                    var pos = target.position;
                    AnimText.Create($"{damage}", pos, fromWorldSpace: true);
                    var direction = (pos - transform.position).normalized;
                    pos -= target.localScale.x * direction;
                    AttackAnimation(pos);
                }
            
                currentIndex++;
            }
        }
    }
}