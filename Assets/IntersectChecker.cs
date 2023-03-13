using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameCore.Battle.RadiusUtils;

namespace BeatRoyale
{
    public class IntersectChecker : MonoBehaviour
    {
        [SerializeField] private Transform transform1;
        [SerializeField] private float radius;
        [SerializeField] private CircleCollider2D collider;
        
        public bool IsIntersected()
        {
            var colliderRadius = collider.radius / X;
            Vector2 center = collider.bounds.center;
            var position = (Vector2) transform1.position;
            Vector2 fullDistance = center - position;
            Vector2 direction = fullDistance.normalized;
            ToPerspective(ref direction);

            Debug.DrawLine(position, position + direction * radius, Color.red);
            direction *= -1;
            Debug.DrawLine(center, center + direction * colliderRadius, Color.red);
            
            return (direction * radius).magnitude + (direction * colliderRadius).magnitude > fullDistance.magnitude;
        }
        
        void Update()
        {
            var trail1 = transform1.GetChild(0).GetComponent<TrailRenderer>();
            var trail2 = collider.transform.GetChild(0).GetComponent<TrailRenderer>();

            if (IsIntersected())
            {
                trail1.startColor = new Color(0.15f, 0.91f, 0.4f);;
                trail1.endColor = new Color(0.15f, 0.91f, 0.4f);;
                
                trail2.startColor = new Color(0.15f, 0.91f, 0.4f);;
                trail2.endColor = new Color(0.15f, 0.91f, 0.4f);;
            }
            else
            {
                trail1.startColor = new Color(0.91f, 0.36f, 0.22f);;
                trail1.endColor = new Color(0.91f, 0.36f, 0.22f);;
                
                trail2.startColor = new Color(0.91f, 0.36f, 0.22f);;
                trail2.endColor = new Color(0.91f, 0.36f, 0.22f);;
            }
        }
    }
}
