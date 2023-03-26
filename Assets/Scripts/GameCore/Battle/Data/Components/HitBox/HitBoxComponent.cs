using System;
using UnityEngine;
using static GameCore.Battle.ObjectsByTransfroms<GameCore.Battle.Data.Components.HitBoxComponent>;

namespace GameCore.Battle.Data.Components
{
    [Serializable]
    internal abstract class HitBoxComponent
    {
        protected Transform transform;
        
        public void Init(Transform transform)
        {
            this.transform = transform;
            Add(transform, this);
            OnInit();
        }

        protected virtual void OnInit(){}

        public void OnDestroy()
        {
            Remove(transform);
        }
        public abstract bool IsIntersected(in Vector2 position, in float radius, out Vector2 point);
    }
}