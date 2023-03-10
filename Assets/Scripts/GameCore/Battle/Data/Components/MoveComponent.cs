using System;
using Battle.Data;
using Battle.Data.GameProperty;
using UnityEngine;

namespace GameCore.Battle.Data.Components
{
    [Serializable]
    internal class MoveComponent
    {
        private FindTargetComponent findTargetComponent;
        private GameObject gameObject;
        private Rigidbody2D rigidbody;
        private CircleCollider2D collider;
        private float speed;
        private static int mask = -1;

        private bool enabled = true;

        public void Init(string entityName, GameObject gameObject, FindTargetComponent findTargetComponent)
        {
            this.gameObject = gameObject;
            rigidbody = gameObject.GetComponent<Rigidbody2D>();
            collider = rigidbody.GetComponent<CircleCollider2D>();
            speed = EntitiesProperties.ByName[entityName][nameof(MoveSpeedGP)].Value;
            this.findTargetComponent = findTargetComponent;

            if (mask == -1)
            {
                mask = LayerMask.GetMask("Cannon");
            }
        }

        public void SetEnabled(bool active)
        {
            enabled = active;
        }

        private void TryByPass(ref Vector2 direction)
        {
            var pos = rigidbody.position;
            var trueRadius = collider.radius * rigidbody.transform.lossyScale.x;
            var hit = Physics2D.Raycast(pos, direction, float.PositiveInfinity, mask);
            var factor = -1;

            if (hit.collider == null)
            {
                Vector2 right = Vector3.Cross(direction, Vector3.forward);
                var newPos = pos + right.normalized * trueRadius;
                hit = Physics2D.Raycast(newPos, direction, float.PositiveInfinity, mask);
                right *= -1;
                
                if (hit.collider != null)
                {
                    var bounds = hit.collider.bounds;
                    var coliderTransform = hit.collider.transform;
                    var hypotenuse = Vector3.Distance(bounds.min, bounds.max);
                    Vector2 point = (Vector2) coliderTransform.position + right.normalized * (hypotenuse + trueRadius * 2);
                    direction = point - pos;
                    return;
                }
            }
            
            if (hit.collider == null)
            {
                Vector2 left = Vector3.Cross(direction, -Vector3.forward);
                var newPos = pos + left.normalized * trueRadius;
                hit = Physics2D.Raycast(newPos, direction, float.PositiveInfinity, mask);
                left *= -1;

                if (hit.collider != null)
                {
                    var bounds = hit.collider.bounds;
                    var coliderTransform = hit.collider.transform;
                    var hypotenuse = Vector3.Distance(bounds.min, bounds.max);
                    Vector2 point = (Vector2) coliderTransform.position + left.normalized * (hypotenuse + trueRadius * 2);
                    direction = point - pos;
                    return;
                }
            }

            if (hit.collider != null)
            {
                var bounds = hit.collider.bounds;
                var coliderTransform = hit.collider.transform;
                var hypotenuse = Vector3.Distance(bounds.min, bounds.max);
                var tangent = Vector3.Cross(direction, hit.normal);
                tangent = Vector3.Cross(direction, tangent) * factor;
                Vector2 point = coliderTransform.position + tangent.normalized * (hypotenuse + trueRadius * 2);
                direction = point - pos;
            }
        }
        
        public void Update()
        {
            if (enabled)
            {
                findTargetComponent.Find();
                var target = findTargetComponent.target;
                if (target != null)
                {
                    var position = rigidbody.position;
                    var direction = (Vector2)target.position - position;
                    TryByPass(ref direction);
                    
                    position += direction.normalized * (speed * Time.deltaTime);
                    rigidbody.position = position;
                }
            }
        }
    }
}