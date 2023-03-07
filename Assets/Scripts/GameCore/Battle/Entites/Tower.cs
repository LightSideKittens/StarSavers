using System.Collections.Generic;
using Battle.Data;
using GameCore.Battle.Data.Components;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GameCore.Battle.Data
{
    public class Tower : SerializedMonoBehaviour
    {
        private static IEnumerable<string> HeroesNames => GameScopes.HeroesNames;
        [SerializeField, ValueDropdown(nameof(HeroesNames))] private string entityName;
        [SerializeField] private HealthComponent healthComponent;
        public static HashSet<Transform> Towers { get; } = new();
        
        private IEnumerable<Transform> Targets
        {
            get
            {
                var units = Unit.ByTransform.Values;

                foreach (var unit in units)
                {
                    if (unit.IsOpponent)
                    {
                        yield return unit.transform;
                    }
                }
            }
        }

        private void Awake()
        {
            Towers.Add(transform);
        }

        private void Start()
        {
            healthComponent.Init(entityName, gameObject, false);
        }

        private void OnDestroy()
        {
            Towers.Remove(transform);
        }
    }
}