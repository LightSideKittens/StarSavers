using UnityEngine;

namespace GameCore.Battle
{
    public static partial class RadiusUtils
    {
        public const float X = 2f;
        public const float Y = 1.42f;
        private const float X2 = X * X;
        private const float Y2 = Y * Y;
        private const float X2Y2 = X2 * Y2;

        public static void ToPerspective(ref Vector2 direction, float radius)
        {
            ToPerspective(ref direction);
            direction *= radius;
        }

        public static void ToPerspective(ref Vector2 direction)
        {
            var k = Mathf.Sqrt(X2Y2 / (direction.x * direction.x * Y2 + direction.y * direction.y * X2));
            direction *= k;
        }
    }
}