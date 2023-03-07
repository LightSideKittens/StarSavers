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
                var units = Unit.ByTransform.Values;

                foreach (var unit in units)
                {
                    if (unit.IsOpponent)
                    {
                        yield return unit.transform;
                    }
                }

                foreach (var cannon in Cannon.Cannons)
                {
                    yield return cannon;
                }
            }
        }

        protected override bool IsOpponent => false;
    }
}