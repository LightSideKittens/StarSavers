using System;
using Battle.Data;
using Battle.Data.GameProperty;
using GameCore.Battle.Data.Components;
using Sirenix.Serialization;
using UnityEngine;
using static GameCore.Battle.RadiusUtils;
using Object = UnityEngine.Object;

namespace GameCore.Battle.Data
{
    [Serializable]
    internal abstract class BaseEffector
    {
        [OdinSerialize] protected FindTargetComponent findTargetComponent;
        [NonSerialized] public string name;
        [field: SerializeField] public int Price { get; private set; }
        protected float radius;
        protected bool isApplied;
        protected Vector2 lastPosition;
        protected SpriteRenderer radiusRenderer;
        private static Sprite circleSprite = Resources.Load<Sprite>("unit-circle");

        public void Init()
        {
            radius = EntitiesProperties.ByName[name][nameof(RadiusGP)].Value;
            OnInit();
        }

        public void DrawRadius(Vector3 position, Color color, int sortingOrder = 0)
        {
            if (radiusRenderer == null)
            {
                radiusRenderer = new GameObject("Radius").AddComponent<SpriteRenderer>();
            }
            
            radiusRenderer.sprite = circleSprite;
            radiusRenderer.color = color;
            radiusRenderer.sortingOrder = sortingOrder;
            Transform transform1 = radiusRenderer.transform;
            transform1.position = position;
            lastPosition = position;
            transform1.localScale = new Vector2(X * radius, Y * radius) * 2;
        }

        public void EndDrawRadius()
        {
            if (!isApplied)
            {
                Object.Destroy(radiusRenderer.gameObject);
            }
        }

        public void Apply()
        {
            isApplied = true;
            OnApply();
        }
        
        protected virtual void OnInit(){}
        protected abstract void OnApply();
    }
}