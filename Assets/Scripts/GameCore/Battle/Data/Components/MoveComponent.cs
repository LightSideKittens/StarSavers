using System;
using Battle.Data;
using Battle.Data.GameProperty;
using UnityEngine;

namespace GameCore.Battle.Data.Components
{
    [Serializable]
    public class MoveComponent
    {
        public event Func<Transform> TargetRequired; 
        private GameObject gameObject;
        private Rigidbody2D rigidbody;
        private float speed;
        [NonSerialized] public Transform target;
        [NonSerialized] public bool enabled = true;

        public void Init(string entityName, GameObject gameObject, Transform target)
        {
            this.gameObject = gameObject;
            rigidbody = gameObject.GetComponent<Rigidbody2D>();
            speed = EntitiesProperties.Config.Properties[entityName][nameof(MoveSpeedGP)].Value / 6;
            this.target = target;
        }

        public void GetTarget()
        {
            target = TargetRequired?.Invoke();
        }

        public void Update()
        {
            if (enabled)
            {
                GetTarget();
                if (target != null)
                {
                    Vector3 position = rigidbody.position;
                    var direction = target.position - position;
                    position += direction.normalized * (speed * Time.deltaTime);
                    rigidbody.position = position;
                }
            }
        }
    }
}