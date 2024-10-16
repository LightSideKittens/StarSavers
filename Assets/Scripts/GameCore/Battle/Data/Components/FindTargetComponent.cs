﻿using System;
using System.Collections.Generic;
using GameCore.Battle.Data.Components.HitBox;
using GameCore.Battle.Data.Components.TargetProviders;
using Sirenix.Serialization;
using UnityEngine;

namespace GameCore.Battle.Data.Components
{
    [Serializable]
    internal class FindTargetComponent
    {
        [NonSerialized] public Transform target;
        [NonSerialized] public bool isOpponent;
        [OdinSerialize] private List<TargetProvider> providers = new() {new AllUnits(), new AllBuildings()};
        private Transform transform;

        public void Init(Transform transform, bool isOpponent)
        {
            this.transform = transform;
            this.isOpponent = isOpponent;
            
            for (int i = 0; i < providers.Count; i++)
            {
                var provider = providers[i];
                provider.findTargetComponent = this;
            }
        }

        public IEnumerable<Transform> FindAll(float radius) => FindAll(transform.position, radius);

        public IEnumerable<Transform> FindAll(Vector2 position, float radius)
        {
            for (int i = 0; i < providers.Count; i++)
            {
                var targets = providers[i].Targets;
                foreach (var target in targets)
                {
                    var hitBox = target.Get<HitBoxComponent>();

                    if (hitBox.IsIntersected(position, radius, out _))
                    {
                        yield return target;
                    }
                }
            }
        }

        public bool Find(Vector2 position, float radius, HashSet<Transform> excepted)
        {
            var distance = radius;
            Transform currentTarget = null;

            for (int i = 0; i < providers.Count; i++)
            {
                var targets = providers[i].Targets;
                foreach (var target in targets)
                {
                    if (!excepted.Contains(target))
                    {
                        var hitBox = target.Get<HitBoxComponent>();

                        if (hitBox.IsIntersected(position, distance, out var point))
                        {
                            var newDistance = Vector2.Distance(point, position);

                            if (distance > newDistance)
                            {
                                currentTarget = target;
                                distance = newDistance;
                            }
                        }
                    }
                }
            }

            target = currentTarget;

            return target != null;
        }

        public bool Find(Vector2 position, float radius)
        {
            var distance = radius;
            Transform currentTarget = null;

            for (int i = 0; i < providers.Count; i++)
            {
                var targets = providers[i].Targets;
                foreach (var target in targets)
                {
                    var hitBox = target.Get<HitBoxComponent>();

                    if (hitBox.IsIntersected(position, distance, out var point))
                    {
                        var newDistance = Vector2.Distance(point, position);

                        if (distance > newDistance)
                        {
                            currentTarget = target;
                            distance = newDistance;
                        }
                    }
                }
            }

            target = currentTarget;

            return target != null;
        }

        public bool Find() => Find(transform.position, 1000);
        public bool Find(HashSet<Transform> excepted) => Find(transform.position, 1000, excepted);
    }
}