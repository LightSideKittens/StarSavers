using System;
using Battle.Data.GameProperty;
using DG.Tweening;
using LSCore.Async;
using UnityEngine;
using Health = GameCore.Battle.Data.Components.HealthComponent;
using static GameCore.Battle.ObjectsByTransfroms<GameCore.Battle.Data.Components.AttackComponent>;

namespace GameCore.Battle.Data.Components
{
    [Serializable]
    internal class AttackComponent
    {
        [SerializeField] protected float duration = 0.1f;
        protected FindTargetComponent findTargetComponent;
        private float attackSpeed;
        protected float damage;
        protected float radius;
        protected Transform transform;
        private int currentIndex;
        private Vector2 lastHitPoint;
        protected float Damage => damage * Buffs;
        public bool IsInRadius { get; private set; }
        public float Radius => radius;
        public Buffs Buffs { get; private set; }
        private Tween attackTween;

        public void Init(Transform transform, FindTargetComponent findTargetComponent)
        {
            this.findTargetComponent = findTargetComponent;
            this.transform = transform;
            var unit = transform.Get<BaseUnit>();
            radius = unit.GetValue<RadiusGP>();
            damage = unit.GetValue<DamageGP>();
            attackSpeed = unit.GetValue<AttackSpeedGP>();
            //DrawRadius(transform, transform.position, radius, new Color(1f, 0.22f, 0.19f, 0.5f));
            attackTween = Wait.InfinityLoop(attackSpeed, OnTactTicked);
            Add(transform, this);
            Buffs = new Buffs();
            OnInit();
        }
        
        protected virtual void OnInit(){}

        public void Update()
        {
            Buffs.Update();
            IsInRadius = CheckInRadius(findTargetComponent.target);
        }

        public void OnDestroy()
        {
            attackTween.Kill();
            Remove(transform);
        }

        public bool CheckInRadius(Transform target)
        {
            if (target != null)
            {
                var hitBox = target.Get<HitBoxComponent>();
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
            if (IsInRadius)
            {
                AttackAnimation();
            }
        }
    }
}