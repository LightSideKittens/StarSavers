using System;
using System.Collections.Generic;
using Battle.Data;
using Battle.Data.GameProperty;
using UnityEngine;

namespace GameCore.Battle.Data.Components
{
    [Serializable]
    public class MoveComponent
    {
        private FindTargetComponent findTargetComponent;
        private GameObject gameObject;
        private Rigidbody2D rigidbody;
        private float speed;

        [NonSerialized] public bool enabled = true;

        public void Init(string entityName, GameObject gameObject, FindTargetComponent findTargetComponent)
        {
            this.gameObject = gameObject;
            rigidbody = gameObject.GetComponent<Rigidbody2D>();
            speed = EntitiesProperties.Config.Properties[entityName][nameof(MoveSpeedGP)].Value / 6;
            this.findTargetComponent = findTargetComponent;
        }

        public void Update()
        {
            if (enabled)
            {
                findTargetComponent.Find();
                var target = findTargetComponent.target;
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