using System;
using System.Collections.Generic;
using Battle.Data;
using Battle.Data.GameProperty;
using BeatRoyale;
using DG.Tweening;
using GameCore.Battle.Data.Components.HitBox;
using UnityEngine;
using static GameCore.Battle.RadiusUtils;

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
        private Vector2 lastHitPoint;
        public bool IsInRadius { get; private set; }
        public float Radius => radius;
        public static Dictionary<Transform, AttackComponent> ByTransform { get; } = new();

        public void Init(string entityName, GameObject gameObject, FindTargetComponent findTargetComponent)
        {
            this.gameObject = gameObject;
            this.findTargetComponent = findTargetComponent;
            transform = gameObject.transform;
            var props = EntitiesProperties.ByName[entityName];
            radius = props[nameof(RadiusGP)].Value;
            damage = props[nameof(DamageGP)].Value;
            attackSpeed = Convert.ToString((int)props[nameof(AttackSpeedGP)].Value, 2);
            listener = TactListener.Listen(-duration).OnTicked(OnTactTicked);
            DrawRadius(transform, transform.position, radius, new Color(1f, 0.22f, 0.19f, 0.5f));
            ByTransform.Add(transform, this);
            OnInit(entityName);
        }
        
        protected virtual void OnInit(string entityName){}

        public void Update()
        {
            IsInRadius = CheckInRadius(findTargetComponent.target);
        }

        public void OnDestroy()
        {
            listener.Ticked -= OnTactTicked;
            listener.Dispose();
            ByTransform.Remove(transform);
        }

        public bool CheckInRadius(Transform target)
        {
            if (target != null)
            {
                var hitBox = HitBoxComponent.ByTransform[target];
                return hitBox.IsIntersected(transform.position, radius, out lastHitPoint);
            }

            return false;
        }

        protected virtual Tween AttackAnimation()
        {
            return transform.DOMove(lastHitPoint, duration).SetLoops(2, LoopType.Yoyo);
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
                    AttackAnimation();
                }
                
                currentIndex++;
            }
        }
    }
}