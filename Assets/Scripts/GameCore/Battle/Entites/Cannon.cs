using System.Collections.Generic;
using GameCore.Battle.Data.Components;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace GameCore.Battle.Data
{
    public class Cannon : SerializedMonoBehaviour
    {
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
            healthComponent.Init("Stoneval", gameObject, true);
            attackComponent.Init("Stoneval", gameObject, findTargetComponent);
            attackComponent.damage /= 2;
            attackComponent.radius *= 2;
        }

        public void Update()
        {
            findTargetComponent.Find();
            attackComponent.Update();
        }

        private void OnDestroy()
        {
            Cannons.Remove(transform);
        }
    }
}