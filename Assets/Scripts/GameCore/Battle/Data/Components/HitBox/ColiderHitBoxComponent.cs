using System;
using UnityEngine;
using static GameCore.Battle.RadiusUtils;

namespace GameCore.Battle.Data.Components.HitBox
{
    [Serializable]
    internal class ColiderHitBoxComponent : HitBoxComponent
    {
        private CircleCollider2D collider;

        protected override void OnInit()
        {
            collider = transform.GetComponent<CircleCollider2D>();
            DrawRadius(collider.transform,
                collider.bounds.center,
                collider.radius / X * collider.transform.lossyScale.x,
                new Color(0.25f, 1f, 0.22f, 0.5f),
                1000);
        }

        public override bool IsIntersected(in Vector2 position, in float radius, out Vector2 point)
        {
            var colliderRadius = collider.radius / X * collider.transform.lossyScale.x;
            Vector2 center = collider.bounds.center;
            Vector2 fullDistance = center - position;
            Vector2 direction = fullDistance.normalized;
            ToPerspective(ref direction);
            point = center - direction * colliderRadius;
            
            return (direction * radius).magnitude  + (direction * colliderRadius).magnitude > fullDistance.magnitude;
        }
    }
}