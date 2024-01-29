/*using System;
using System.Collections.Generic;
using UnityEngine;

namespace LSCore.BattleModule
{
    [Serializable]
    internal class AllBuildings : TargetProvider
    {
        public override IEnumerable<Transform> Targets
        {
            get
            {
                if (findTargetComponent.isOpponent)
                {
                    foreach (var tower in Tower.Towers)
                    {
                        yield return tower;
                    }
                }
                else
                {
                    foreach (var cannon in Cannon.Cannons)
                    {
                        yield return cannon;
                    }
                }
            }
        }
    }
}*/