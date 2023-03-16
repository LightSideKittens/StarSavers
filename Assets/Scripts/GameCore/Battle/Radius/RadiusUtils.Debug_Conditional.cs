using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

#if DEBUG
using static Core.ConfigModule.BaseConfig<BeatRoyale.DebugData>;
#endif

namespace GameCore.Battle
{
    public static partial class RadiusUtils
    {
        [Conditional("DEBUG")]
        public static void SetActiveRadiuses(bool active)
        {
            foreach (var radius in radiuses.Values)
            {
                for (int i = 0; i < radius.Count; i++)
                {
                    radius[i].SetActive(active);
                }
            }
        }

        [Conditional("DEBUG")]
        public static void DrawRadius(Transform transform, Vector3 position, float radius, Color color, int sortingOrder = 0)
        {
#if DEBUG
            var spriteRenderer = new GameObject("Radius").AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = circleSprite;
            spriteRenderer.color = color;
            spriteRenderer.sortingOrder = sortingOrder;
            Transform transform1 = spriteRenderer.transform;
            transform1.position = position;
            transform1.localScale = new Vector2(X * radius, Y * radius) * 2;
            transform1.SetParent(transform);
            spriteRenderer.gameObject.SetActive(Config.needShowRadius);

            if (!radiuses.TryGetValue(transform, out var list))
            {
                list = new List<GameObject>();
                radiuses.Add(transform, list);
            }
            
            list.Add(spriteRenderer.gameObject);
#endif
        }
    }
}