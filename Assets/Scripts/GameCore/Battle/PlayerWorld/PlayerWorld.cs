using System.Collections.Generic;
using GameCore.Battle.Data;
using UnityEngine;

namespace Battle
{
    public class PlayerWorld : BasePlayerWorld<PlayerWorld>
    {
        protected override IEnumerable<Transform> Targets
        {
            get
            {
                var units = Unit.ByTransform;

                foreach (var unit in units)
                {
                    if (unit.Value.IsOpponent)
                    {
                        yield return unit.Value.transform;
                    }
                }
            }
        }

        protected override bool IsOpponent => false;
    }
}