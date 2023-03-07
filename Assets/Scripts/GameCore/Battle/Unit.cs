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
        public MoveComponent moveComponent;
        public AttackComponent attackComponent;
        public HealthComponent healthComponent;
        public bool IsOpponent { get; private set; }
        public static Dictionary<Transform, Unit> ByTransform { get; } = new();
        private IEnumerable<Transform> targets;

        public void Init(IEnumerable<Transform> targets, bool isOpponent)
        {
            this.targets = targets;
            IsOpponent = isOpponent;
            ByTransform.Add(transform, this);
            moveComponent.Init(entityName, gameObject, GetNearest());
            healthComponent.Init(entityName, gameObject);
            attackComponent.Init(entityName, gameObject, moveComponent, healthComponent);
            moveComponent.TargetRequired += GetNearest;
        }

        public void Run()
        {
            attackComponent.Update();
            healthComponent.Update();
        }

        public void FixedRun()
        {
            moveComponent.Update();
        }

        private void OnDestroy()
        {
            ByTransform.Remove(transform);
            attackComponent.OnDestroy();
            moveComponent.TargetRequired -= GetNearest;
        }

        private Transform GetNearest()
        {
            var distance = float.MaxValue;
            Transform currentTarget = null;

            foreach (var target in targets)
            {
                var newDistance = Vector2.Distance(target.position, transform.position);

                if (distance > newDistance)
                {
                    currentTarget = target;
                    distance = newDistance;
                }
            }

            return currentTarget;
        }
    }
}