using System.Collections.Generic;
using Battle.Data;
using GameCore.Battle.Data.Components;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GameCore.Battle.Data
{
    public class Unit : SerializedMonoBehaviour
    {
        private static IEnumerable<string> Entities => GameScopes.EntitiesNames;
        
        [SerializeField, ValueDropdown(nameof(Entities))] private string entityName;
        [SerializeField] private MoveComponent moveComponent;
        [SerializeField] private FindTargetComponent findTargetComponent;
        [SerializeField] private AttackComponent attackComponent;
        [SerializeField] private HealthComponent healthComponent;
        public bool IsOpponent { get; private set; }
        public static Dictionary<Transform, Unit> ByTransform { get; } = new();

        public void Init(IEnumerable<Transform> targets, bool isOpponent)
        {
            IsOpponent = isOpponent;
            ByTransform.Add(transform, this);
            findTargetComponent.Init(gameObject, targets);
            moveComponent.Init(entityName, gameObject, findTargetComponent);
            healthComponent.Init(entityName, gameObject, IsOpponent);
            attackComponent.Init(entityName, gameObject, findTargetComponent);
        }

        public void Run()
        {
            attackComponent.Update();
            moveComponent.enabled = !attackComponent.IsInRadius;
            healthComponent.Update();
        }

        public void FixedRun()
        {
            moveComponent.Update();
        }

        private void OnDestroy()
        {
            attackComponent.OnDestroy();
            healthComponent.OnDestroy();
            ByTransform.Remove(transform);
        }
    }
}