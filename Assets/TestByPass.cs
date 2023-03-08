using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BeatRoyale
{
    public class TestByPass : MonoBehaviour
    {
        private Vector2 TryByPass(Vector2 direction)
        {
            var hit = Physics2D.Raycast(transform.position, direction, direction.magnitude);
            
            if (hit.collider != null)
            {
                var colider = hit.collider;
                var radius = 1;
                var tangent = Vector3.Cross(direction, hit.normal);
                tangent = Vector3.Cross(direction, tangent) * -1;
                Debug.Log($"[Malvis] {tangent}");
                var position = colider.transform.position;
                Debug.DrawLine(transform.position, hit.point, Color.magenta);
                Debug.DrawRay(position, hit.normal, Color.red);
                Debug.DrawRay(position, tangent.normalized * (radius * colider.transform.lossyScale.x), Color.yellow); 
                return position + tangent.normalized * (radius * colider.transform.lossyScale.x);
            }

            return default;
        }
        
        void Update()
        {
            if (Input.GetMouseButton(0))
            {
                var point = TryByPass( (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition) - (Vector2)transform.position);
                Debug.DrawLine(transform.position, point, Color.white);
            }
        }
    }
}
