using System;
using System.Collections.Generic;
using GameCore.Battle.Data.Components;
using GameCore.Battle.Data.Components.HitBox;
using Sirenix.Serialization;
using UnityEngine;
using static GameCore.Battle.ObjectsByTransfroms<GameCore.Battle.Data.Unit>;

namespace GameCore.Battle.Data
{
    public class Unit : BaseUnit
    {
        public event Action Destroyed;
        public static Dictionary<string, Dictionary<Transform, Unit>> All { get; } = new();
        [OdinSerialize] private MoveComponent moveComponent = new();
        [OdinSerialize] private FindTargetComponent findTargetComponent = new ();
        [OdinSerialize] private AttackComponent attackComponent = new();
        [OdinSerialize] private HealthComponent healthComponent = new();
        [OdinSerialize] private HitBoxComponent hitBoxComponent = new ColiderHitBoxComponent();
        
        private float radius;

        public override void Init(string userId)
        {
            base.Init(userId);
            var transform = this.transform;
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

        public void Reset()
        {
            attackComponent.Buffs.Reset();
            moveComponent.Buffs.Reset();
            healthComponent.Reset();
        }

        public void Enable()
        {
            All[UserId].Add(transform, this);
            gameObject.SetActive(true);
            attackComponent.Enable();
        }
        
        public void Disable()
        {
            All[UserId].Remove(transform);
            gameObject.SetActive(false);
            attackComponent.Disable();
        }

        public override void Destroy()
        {
            base.Destroy();
            All[UserId].Remove(transform);
            hitBoxComponent.Destroy();
            attackComponent.Destroy();
            healthComponent.Destroy();
            moveComponent.Destroy();
            Remove(transform);
            Destroyed?.Invoke();
        }
    }
}