using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameCore.Battle.Data.Components
{
    [Serializable]
    public class FindTargetComponent
    {
        [NonSerialized] public Transform target;
        private IEnumerable<Transform> targets;
        private GameObject gameObject;
        private Transform transform;

        public void Init(GameObject gameObject, IEnumerable<Transform> targets)
        {
            this.gameObject = gameObject;
            this.targets = targets;
            transform = gameObject.transform;
        }

        public void Find()
        {
            var distance = float.MaxValue;
            Transform currentTarget = null;

            foreach (var target in targets)
            {
                var newDistance = Vector2.Distance(target.position, transform.position);

                if (distance > newDistance)
                {
                    currentTarget = target;
                    distance = newDistance;
                }
            }

            target = currentTarget;
        }
    }
}