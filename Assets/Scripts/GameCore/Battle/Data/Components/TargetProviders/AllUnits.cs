using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameCore.Battle.Data.Components.TargetProviders
{
    [Serializable]
    internal class AllUnits : TargetProvider
    {
        public override IEnumerable<Transform> Targets 
        {
            get
            {
                var units = Unit.ByTransform.Values;

                foreach (var unit in units)
                {
                    if (unit.IsOpponent != isOpponent)
                    {
                        yield return unit.transform;
                    }
                }
            }
        }
    }
}