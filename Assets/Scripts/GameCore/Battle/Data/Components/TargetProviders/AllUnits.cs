using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameCore.Battle.Data.Components.TargetProviders
{
    using Unit = ObjectsByTransfroms<Unit>;
    
    [Serializable]
    internal class AllUnits : TargetProvider
    {
        public override IEnumerable<Transform> Targets 
        {
            get
            {
                var units = Unit.All;

                foreach (var unit in units)
                {
                    if (unit.IsOpponent != findTargetComponent.isOpponent)
                    {
                        yield return unit.transform;
                    }
                }
            }
        }
    }
}