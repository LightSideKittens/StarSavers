using System.Collections.Generic;
using Battle.Data;
using GameCore.Battle.Data.Components;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace GameCore.Battle.Data
{
    public class Unit : SerializedMonoBehaviour
    {
        private static IEnumerable<string> Entities => GameScopes.EntitiesNames;
        
        [SerializeField, ValueDropdown(nameof(Entities))] private string entityName;
        [field: SerializeField] public int Price { get; private set; }
        
        [OdinSerialize] private MoveComponent moveComponent = new();
        [OdinSerialize] private FindTargetComponent findTargetComponent = new ();
        [OdinSerialize] private AttackComponent attackComponent = new();
        [OdinSerialize] internal HealthComponent healthComponent = new();
        
        public bool IsOpponent { get; private set; }
        public static Dictionary<Transform, Unit> ByTransform { get; } = new();
        private float radius;

        public void Init(bool isOpponent)
        {
            IsOpponent = isOpponent;
            ByTransform.Add(transform, this);
            findTargetComponent.Init(gameObject, IsOpponent);
            moveComponent?.Init(entityName, gameObject, findTargetComponent);
            healthComponent.Init(entityName, gameObject, IsOpponent);
            attackComponent.Init(entityName, gameObject, findTargetComponent);
        }

        private void OnDrawGizmosSelected()
        {
            var color = Color.green;
            color.a = 0.5f;
            Gizmos.color = color;
            
            Gizmos.DrawSphere(transform.position, attackComponent.radius);
        }

        public void Run()
        {
            attackComponent.Update();
            moveComponent?.SetEnabled(!attackComponent.IsInRadius);
            healthComponent.Update();
        }

        public void FixedRun()
        {
            moveComponent?.Update();
        }

        private void OnDestroy()
        {
            attackComponent.OnDestroy();
            healthComponent.OnDestroy();
            ByTransform.Remove(transform);
        }
    }
}