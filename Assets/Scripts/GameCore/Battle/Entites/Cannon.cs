using System.Collections.Generic;
using Battle;
using GameCore.Battle.Data.Components;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GameCore.Battle.Data
{
    public class Cannon : SerializedMonoBehaviour
    {
        [SerializeField] private FindTargetComponent findTargetComponent;
        [SerializeField] private CannonAttackComponent attackComponent;
        [SerializeField] private HealthComponent healthComponent;
        public static HashSet<Transform> Cannons { get; } = new();
        
        private IEnumerable<Transform> Targets
        {
            get
            {
                var units = Unit.ByTransform.Values;

                foreach (var unit in units)
                {
                    if (!unit.IsOpponent)
                    {
                        yield return unit.transform;
                    }
                }
            }
        }

        private void Awake()
        {
            Cannons.Add(transform);
        }

        private void Start()
        {
            findTargetComponent.Init(gameObject, Targets);
            healthComponent.Init("Stoneval", gameObject, true);
            attackComponent.Init("Stoneval", gameObject, findTargetComponent);
        }

        public void Update()
        {
            attackComponent.Update();
        }

        private void OnDestroy()
        {
            Cannons.Remove(transform);
        }
    }
}