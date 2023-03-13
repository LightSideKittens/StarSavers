using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameCore.Battle.Data.Components.HitBox
{
    [Serializable]
    internal abstract class HitBoxComponent
    {
        public static Dictionary<Transform, HitBoxComponent> ByTransform { get; } = new();
        protected GameObject gameObject;

        public void Init(GameObject gameObject)
        {
            this.gameObject = gameObject;
            ByTransform.Add(gameObject.transform, this);
            OnInit();
        }

        protected virtual void OnInit(){}

        public void OnDestroy()
        {
            ByTransform.Remove(gameObject.transform);
        }
        public abstract bool IsIntersected(in Vector2 position, in float radius, out Vector2 point);
    }
}