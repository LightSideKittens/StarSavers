using System;
using System.Collections.Generic;
using UnityEngine;

namespace Battle.Data.Components.TargetProviders
{
    [Serializable]
    internal class AllEnemies : TargetProvider
    {
        public override IEnumerable<Transform> Targets 
        {
            get
            {
                foreach (var unit in OpponentWorld.ActiveUnits)
                {
                    yield return unit.transform;
                }
            }
        }
    }
}