using UnityEngine;

namespace GameCore.Battle
{
    internal static class TransformExtensions
    {
        public static T Get<T>(this Transform target)
        {
            return ObjectsByTransfroms<T>.Get(target);
        }

        public static bool TryGet<T>(this Transform target, out T result)
        {
            return ObjectsByTransfroms<T>.TryGet(target, out result);
        }
    }
}