using System;
using Battle.Data;
using Battle.Data.GameProperty;
using BeatRoyale;
using DG.Tweening;
using GameCore.Battle.Data.Components.HitBox;
using UnityEngine;
using Health = GameCore.Battle.Data.Components.HealthComponent;
using static GameCore.Battle.ObjectsByTransfroms<GameCore.Battle.Data.Components.AttackComponent>;
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
        private int tickedIndex;
        private TactListener listener;
        private Vector2 lastHitPoint;
        protected float Damage => damage * Buffs;
        public bool IsInRadius { get; private set; }
        public float Radius => radius;
        public Buffs Buffs { get; private set; }

        public void Init(string entityName, GameObject gameObject, FindTargetComponent findTargetComponent)
        {
            this.gameObject = gameObject;
            this.findTargetComponent = findTargetComponent;
            transform = gameObject.transform;
            var props = EntiProps.ByName[entityName];
            radius = props[nameof(RadiusGP)].Value;
            damage = props[nameof(DamageGP)].Value;
            attackSpeed = Convert.ToString((int)props[nameof(AttackSpeedGP)].value, 2);
            listener = TactListener.Listen(-duration).OnTicked(OnTactTicked);
            DrawRadius(transform, transform.position, radius, new Color(1f, 0.22f, 0.19f, 0.5f));
            Add(transform, this);
            Buffs = new Buffs();
            OnInit(entityName);
        }
        
        protected virtual void OnInit(string entityName){}

        public void Update()
        {
            Buffs.Update();
            IsInRadius = CheckInRadius(findTargetComponent.target);
        }

        public void OnDestroy()
        {
            listener.Ticked -= OnTactTicked;
            listener.Dispose();
            Remove(transform);
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
            var lastPos = transform.position;
            return transform.DOMove(lastHitPoint, duration).OnComplete(() =>
            {
                var target = findTargetComponent.target;
                target.Get<Health>().TakeDamage(Damage);
                transform.DOMove(lastPos, duration);
            });
        }

        private void OnTactTicked()
        {
            tickedIndex++;
            
            if (IsInRadius)
            {
                currentIndex %= attackSpeed.Length;
                var step = attackSpeed[currentIndex];

                if (step == '1')
                {
                    findTargetComponent.Find();
                    AttackAnimation();
                }
                
                currentIndex++;
            }
        }
    }
}