using System.Collections.Generic;
using Battle.Data;
using GameCore.Battle.Data.Components;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace GameCore.Battle.Data
{
    public class Cannon : SerializedMonoBehaviour
    {
        private static IEnumerable<string> EntitiesNames => GameScopes.EntitiesNames;
        [ValueDropdown("EntitiesNames"), SerializeField] 
        private string entityName;
        
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
            healthComponent.OnDestroy();
            attackComponent.OnDestroy();
            Cannons.Remove(transform);
        }
    }
}