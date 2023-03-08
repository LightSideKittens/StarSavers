using System;
using System.Collections.Generic;
using System.Linq;
using GameCore.Battle.Data.Components.TargetProviders;
using Sirenix.Serialization;
using UnityEngine;

namespace GameCore.Battle.Data.Components
{
    [Serializable]
    internal class FindTargetComponent
    {
        [NonSerialized] public Transform target;
        [OdinSerialize] private List<TargetProvider> providers = new() {new AllUnits(), new AllBuildings()};
        private GameObject gameObject;
        private Transform transform;

        public void Init(GameObject gameObject, bool isOpponent)
        {
            this.gameObject = gameObject;
            transform = gameObject.transform;

            for (int i = 0; i < providers.Count; i++)
            {
                var provider = providers[i];
                provider.isOpponent = isOpponent;
            }
        }

        public bool Find()
        {
            var distance = float.MaxValue;
            Transform currentTarget = null;

            for (int i = 0; i < providers.Count; i++)
            {
                var targets = providers[i].Targets;
                foreach (var target in targets)
                {
                    var newDistance = Vector2.Distance(target.position, transform.position);

                    if (distance > newDistance)
                    {
                        currentTarget = target;
                        distance = newDistance;
                    }
                }
            }

            target = currentTarget;

            return target != null;
        }
    }
}