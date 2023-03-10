using System;
using Battle.Data;
using Battle.Data.GameProperty;
using BeatRoyale;
using Common.SingleServices;
using DG.Tweening;
using MusicEventSystem.Configs;
using UnityEngine;

namespace GameCore.Battle.Data.Components
{
    [Serializable]
    internal class AttackComponent
    {
        [SerializeField] protected float duration = 0.1f;
        protected FindTargetComponent findTargetComponent;
        private string attackSpeed;
        protected float damage;
        protected float radius;
        protected GameObject gameObject;
        protected Transform transform;
        private int currentIndex;
        private TactListener listener;
        public bool IsInRadius { get; private set; }

        public void Init(string entityName, GameObject gameObject, FindTargetComponent findTargetComponent)
        {
            this.gameObject = gameObject;
            this.findTargetComponent = findTargetComponent;
            transform = gameObject.transform;
            var props = EntitiesProperties.ByName[entityName];
            radius = props[nameof(RadiusGP)].Value / 4;
            damage = props[nameof(DamageGP)].Value;
            attackSpeed = Convert.ToString((int)props[nameof(AttackSpeedGP)].Value, 2);
            listener = TactListener.Listen(-duration).OnTicked(OnTactTicked);
        }

        public void Update()
        {
            IsInRadius = CheckInRadius(findTargetComponent.target);
        }

        public void OnDestroy()
        {
            listener.Ticked -= OnTactTicked;
            listener.Dispose();
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

        protected virtual Tween AttackAnimation(Vector2 targetPosition)
        {
            return transform.DOMove(targetPosition, duration).SetLoops(2, LoopType.Yoyo);
        }

        private void OnTactTicked()
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
                    pos -= (target.localScale.x + transform.localScale.x) * direction;
                    AttackAnimation(pos);
                }
            
                currentIndex++;
            }
        }
    }
}