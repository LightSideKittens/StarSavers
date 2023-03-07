using System;
using Battle.Data;
using Battle.Data.GameProperty;
using Common.SingleServices;
using DG.Tweening;
using MusicEventSystem.Configs;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GameCore.Battle.Data.Components
{
    [Serializable]
    public class AttackComponent
    {
        private MoveComponent moveComponent;
        private HealthComponent healthComponent;
        private string attackSpeed;
        private float damage;
        private float radius;
        private GameObject gameObject;
        private Transform transform;
        private bool isInRadius;
        private int currentIndex;

        public void Init(string entityName, GameObject gameObject, MoveComponent moveComponent, HealthComponent healthComponent)
        {
            this.gameObject = gameObject;
            this.moveComponent = moveComponent;
            this.healthComponent = healthComponent;
            transform = gameObject.transform;
            var props = EntitiesProperties.Config.Properties[entityName];
            radius = props[nameof(RadiusGP)].Value / 4;
            damage = props[nameof(DamageGP)].Value;
            attackSpeed = Convert.ToString((int)props[nameof(AttackSpeedGP)].Value, 2);
            MusicData.BPMReached += OnBpmReached;
        }

        public void Update()
        {
            isInRadius = IsInRadius(moveComponent.target);
            moveComponent.enabled = !isInRadius;
        }

        public void OnDestroy()
        {
            MusicData.BPMReached -= OnBpmReached;
        }

        private bool IsInRadius(Transform target)
        {
            if (target != null)
            {
                var distance = Vector2.Distance(target.position, transform.position);
                return distance < radius;
            }

            return false;
        }

        private void OnBpmReached()
        {
            if (isInRadius)
            {
                var step = attackSpeed[currentIndex % attackSpeed.Length];

                if (step == '1')
                {
                    Unit.ByTransform[moveComponent.target].healthComponent.TakeDamage(damage);
                    moveComponent.GetTarget();
                    var pos = moveComponent.target.position;
                    AnimText.Create($"{damage}", pos, fromWorldSpace: true);
                    transform.DOMove(pos, 0.1f).SetLoops(2, LoopType.Yoyo);
                }
            
                currentIndex++;
            }
        }
    }
}