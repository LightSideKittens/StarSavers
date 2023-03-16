#if DEBUG
using System.Collections.Generic;
using GameCore.Battle.Data;
using UnityEngine;

namespace GameCore.Battle
{
    public static partial class RadiusUtils
    {
        private static Sprite circleSprite = Resources.Load<Sprite>("unit-circle");
        private static Dictionary<Transform, List<GameObject>> radiuses = new();
        
        static RadiusUtils()
        {
            Unit.Destroyed += OnUnitDestroyed;
            Tower.Destroyed += OnUnitDestroyed;
            Cannon.Destroyed += OnUnitDestroyed;
            OnTool();
        }

        private static void OnUnitDestroyed(Transform obj)
        {
            radiuses.Remove(obj);
        }

        static partial void OnTool();
    }
}
#endif