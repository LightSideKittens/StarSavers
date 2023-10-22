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
        protected Vector2 lastHitPoint;
        protected float Damage => damage * Buffs;
        public bool IsInRadius { get; private set; }
        public float Radius => radius;
        public Buffs Buffs { get; private set; }
        protected Tween attackLoopEmiter;

        public void Init(Transform transform, FindTargetComponent findTargetComponent)
        {
            this.findTargetComponent = findTargetComponent;
            this.transform = transform;
            var unit = transform.Get<BaseUnit>();
            radius = unit.GetValue<RadiusGP>();
            damage = unit.GetValue<DamageGP>();
            attackSpeed = unit.GetValue<AttackSpeedGP>();
            //DrawRadius(transform, transform.position, radius, new Color(1f, 0.22f, 0.19f, 0.5f));
            Add(transform, this);
            Buffs = new Buffs();
            OnInit();
        }
        
        protected virtual void OnInit(){}

        public void Enable()
        {
            attackLoopEmiter = Wait.InfinityLoop(attackSpeed, OnTactTicked);
        }

        public void Disable() => attackLoopEmiter.Kill();

        public void Update()
        {
            Buffs.Update();
        }

        public void Destroy()
        {
            attackLoopEmiter.Kill();
            Remove(transform);
        }

        public bool CheckInRadius()
        {
            var target = findTargetComponent.target;
            if (target.TryGet<HitBoxComponent>(out var hitBox))
            {
                IsInRadius = hitBox.IsIntersected(transform.position, radius, out lastHitPoint);
                return IsInRadius;
            }

            return false;
        }

        protected virtual Tween AttackAnimation()
        {
            var lastPos = transform.position;
            return transform.DOMove(lastHitPoint, duration).OnComplete(() =>
            {
                var target = findTargetComponent.target;
                TryApplyDamage(target);
                transform.DOMove(lastPos, duration);
            });
        }

        protected void TryApplyDamage(Transform target)
        {
            if (target != null && target.TryGet<Health>(out var health))
            {
                health.TakeDamage(Damage);
            }
        }
        
        private void OnTactTicked()
        {
            if (findTargetComponent.Find(radius))
            {
                if (CheckInRadius())
                {
                    AttackAnimation();
                }
            }
        }
    }
}