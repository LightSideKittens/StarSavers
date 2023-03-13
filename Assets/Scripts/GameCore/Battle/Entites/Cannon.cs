using System;
using System.Collections.Generic;
using Battle.Data;
using GameCore.Battle.Data.Components;
using GameCore.Battle.Data.Components.HitBox;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace GameCore.Battle.Data
{
    public class Cannon : SerializedMonoBehaviour
    {
        public static event Action<Transform> Destroyed;
        private static IEnumerable<string> EntitiesNames => GameScopes.EntitiesNames;
        [ValueDropdown("EntitiesNames"), SerializeField] 
        private string entityName;
        
        [OdinSerialize] private HitBoxComponent hitBoxComponent = new ColiderHitBoxComponent();
        [OdinSerialize] private FindTargetComponent findTargetComponent = new();
        [SerializeField] private CannonAttackComponent attackComponent;
        [SerializeField] private HealthComponent healthComponent;
        public static HashSet<Transform> Cannons { get; } = new();

        private void Awake()
        {
            Cannons.Add(transform);
        }

        private void Start()
        {
            hitBoxComponent.Init(gameObject);
            findTargetComponent.Init(gameObject, true);
            healthComponent.Init(entityName, gameObject, true);
            attackComponent.Init(entityName, gameObject, findTargetComponent);
        }

        public void Update()
        {
            findTargetComponent.Find();
            attackComponent.Update();
        }

        private void OnDestroy()
        {
            hitBoxComponent.OnDestroy();
            healthComponent.OnDestroy();
            attackComponent.OnDestroy();
            Cannons.Remove(transform);
            Destroyed?.Invoke(transform);
        }
    }
}