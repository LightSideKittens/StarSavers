using System;
using GameCore.Battle.Data.Components;
using GameCore.Battle.Data.Components.HitBox;
using Sirenix.Serialization;
using static GameCore.Battle.ObjectsByTransfroms<GameCore.Battle.Data.Unit>;

namespace GameCore.Battle.Data
{
    public class Unit : BaseUnit
    {
        public event Action Destroyed;
        [OdinSerialize] private MoveComponent moveComponent = new();
        [OdinSerialize] private FindTargetComponent findTargetComponent = new ();
        [OdinSerialize] private AttackComponent attackComponent = new();
        [OdinSerialize] private HealthComponent healthComponent = new();
        [OdinSerialize] private HitBoxComponent hitBoxComponent = new ColiderHitBoxComponent();
        
        private float radius;

        public override void Init(string userId)
        {
            base.Init(userId);
            Add(transform, this);

            hitBoxComponent.Init(transform);
            findTargetComponent.Init(transform, IsOpponent);
            moveComponent.Init(transform, findTargetComponent);
            healthComponent.Init(transform, IsOpponent);
            attackComponent.Init(transform, findTargetComponent);
        }

        public void Run()
        {
            attackComponent.Update();
            moveComponent.SetEnabled(!attackComponent.IsInRadius);
            healthComponent.Update();
        }

        public void FixedRun()
        {
            moveComponent.Update();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            hitBoxComponent.OnDestroy();
            attackComponent.OnDestroy();
            healthComponent.OnDestroy();
            moveComponent.OnDestroy();
            Remove(transform);
            Destroyed?.Invoke();
        }
    }
}