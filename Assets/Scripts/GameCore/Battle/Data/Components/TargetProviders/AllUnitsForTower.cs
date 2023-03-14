using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameCore.Battle.Data.Components.TargetProviders
{
    [Serializable]
    internal class AllUnitsForTower : TargetProvider
    {
        [SerializeField] private float yPositionThreshold;
        public override IEnumerable<Transform> Targets 
        {
            get
            {
                var units = Unit.ByTransform.Values;

                foreach (var unit in units)
                {
                    if (unit.IsOpponent != findTargetComponent.isOpponent)
                    {
                        if (unit.transform.position.y < yPositionThreshold)
                        {
                            yield return unit.transform;
                        }
                    }
                }
            }
        }
    }
}