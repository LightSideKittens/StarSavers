using System;
using System.Collections.Generic;
using GameCore.Battle.Data;
using UnityEditor;
using UnityEngine;
using UnityToolbarExtender;

namespace GameCore.Battle
{
    [InitializeOnLoad]
    public static class RadiusUtils
    {
        public const float X = 2f;
        public const float Y = 1.42f;
        private const float X2 = X * X;
        private const float Y2 = Y * Y;
        private const float X2Y2 = X2 * Y2;
        private static Sprite circleSprite = Resources.Load<Sprite>("unit-circle");
        private static bool showRadius;
        private static Dictionary<Transform, List<GameObject>> radiuses = new();

        static RadiusUtils()
        {
            Unit.Destroyed += OnUnitDestroyed;
            Tower.Destroyed += OnUnitDestroyed;
            Cannon.Destroyed += OnUnitDestroyed;
            ToolbarExtender.LeftToolbarGUI.Add(OnToolbarLeftGUI);
        }

        private static void OnUnitDestroyed(Transform obj)
        {
            radiuses.Remove(obj);
        }

        static void OnToolbarLeftGUI()
        {
            if (showRadius != GUILayout.Toggle(showRadius, "Show Radius"))
            {
                showRadius = !showRadius;

                foreach (var radius in radiuses.Values)
                {
                    for (int i = 0; i < radius.Count; i++)
                    {
                        radius[i].SetActive(showRadius);
                    }
                }
            }
        }
        
        public static void DrawRadius(Transform transform, Vector3 position, float radius, Color color, int sortingOrder = 0)
        {
            var spriteRenderer = new GameObject("Radius").AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = circleSprite;
            spriteRenderer.color = color;
            spriteRenderer.sortingOrder = sortingOrder;
            Transform transform1 = spriteRenderer.transform;
            transform1.position = position;
            transform1.localScale = new Vector2(X * radius, Y * radius) * 2;
            transform1.SetParent(transform);
            spriteRenderer.gameObject.SetActive(showRadius);

            if (!radiuses.TryGetValue(transform, out var list))
            {
                list = new List<GameObject>();
                radiuses.Add(transform, list);
            }
            
            list.Add(spriteRenderer.gameObject);
        }

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