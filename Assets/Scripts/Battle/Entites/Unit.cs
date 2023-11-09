using System;
using System.Collections.Generic;
using Battle.Data.Components;
using Battle.Data.Components.HitBox;
using Sirenix.Serialization;
using UnityEngine;
using static Battle.ObjectsByTransfroms<Battle.Data.Unit>;

namespace Battle.Data
{
    public partial class Unit : BaseUnit
    {
        public static Dictionary<string, Dictionary<Transform, Unit>> ByWorld { get; } = new();

        public static IEnumerable<Unit> All
        {
            get
            {
                foreach (var world in ByWorld.Values)
                {
                    foreach (var unit in world.Values)
                    {
                        yield return unit;
                    }
                }
            }
        }

#if UNITY_EDITOR
        protected override string GroupName => "AllUnits";
#endif
        public event Action Destroyed;

        [OdinSerialize] private MoveComponent moveComponent = new();
        [OdinSerialize] private FindTargetComponent findTargetComponent = new ();
        [OdinSerialize] private AttackComponent attackComponent = new();
        [OdinSerialize] private HealthComponent healthComponent = new();
        [OdinSerialize] private HitBoxComponent hitBoxComponent = new ColiderHitBoxComponent();

        public bool IsEnabled => ByWorld[UserId][transform].isEnabled;
        private bool isEnabled;
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
            isEnabled = true;
            ByWorld[UserId].Add(transform, this);
            gameObject.SetActive(true);
            attackComponent.Enable();
        }
        
        public void Disable()
        {
            isEnabled = false;
            ByWorld[UserId].Remove(transform);
            gameObject.SetActive(false);
            attackComponent.Disable();
        }

        public override void Destroy()
        {
            base.Destroy();
            isEnabled = false;
            ByWorld[UserId].Remove(transform);
            hitBoxComponent.Destroy();
            attackComponent.Destroy();
            healthComponent.Destroy();
            moveComponent.Destroy();
            Remove(transform);
            Destroyed?.Invoke();
        }
    }
}