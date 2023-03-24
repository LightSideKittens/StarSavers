using System;
using System.Collections.Generic;
using Battle.Data;
using GameCore.Battle.Data.Components;
using GameCore.Battle.Data.Components.HitBox;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using static GameCore.Battle.ObjectsByTransfroms<GameCore.Battle.Data.Unit>;

namespace GameCore.Battle.Data
{
    public class Unit : SerializedMonoBehaviour
    {
        public static event Action<Transform> Destroyed;
        private static IEnumerable<string> Entities => GameScopes.EntitiesNames;
        
        [SerializeField, ValueDropdown(nameof(Entities))] private string entityName;
        [field: SerializeField] public int Price { get; private set; }
        
        [OdinSerialize] private MoveComponent moveComponent = new();
        [OdinSerialize] private FindTargetComponent findTargetComponent = new ();
        [OdinSerialize] private AttackComponent attackComponent = new();
        [OdinSerialize] private HealthComponent healthComponent = new();
        [OdinSerialize] private HitBoxComponent hitBoxComponent = new ColiderHitBoxComponent();
        
        public bool IsOpponent { get; private set; }
        private float radius;

        public void Init(bool isOpponent)
        {
            IsOpponent = isOpponent;
            Add(transform, this);

            hitBoxComponent.Init(gameObject);
            findTargetComponent.Init(gameObject, IsOpponent);
            moveComponent?.Init(entityName, gameObject, findTargetComponent);
            healthComponent.Init(entityName, gameObject, IsOpponent);
            attackComponent.Init(entityName, gameObject, findTargetComponent);
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
            hitBoxComponent.OnDestroy();
            attackComponent.OnDestroy();
            healthComponent.OnDestroy();
            moveComponent?.OnDestroy();
            Remove(transform);
            Destroyed?.Invoke(transform);
        }
    }
}