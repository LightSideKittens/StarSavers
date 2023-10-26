using Battle.Data.GameProperty;
using GameCore.Battle.Data;
using UnityEngine;

namespace GameCore.Battle
{
    internal static class TransformExtensions
    {
        public static T Get<T>(this Transform target)
        {
            return ObjectsByTransfroms<T>.Get(target);
        }
        
        public static float GetValue<T>(this Transform transform) where T : BaseGameProperty
        {
            var unit = ObjectsByTransfroms<BaseUnit>.Get(transform);
            return FloatAndPercent.GetValue<T>(unit.Props);
        }

        public static bool TryGet<T>(this Transform target, out T result)
        {
            return ObjectsByTransfroms<T>.TryGet(target, out result);
        }
    }
}