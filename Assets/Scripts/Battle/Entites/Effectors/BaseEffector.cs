using System;
using Battle.Data;
using Battle.Data.GameProperty;
using GameCore.Battle.Data.Components;
using Sirenix.Serialization;
using UnityEngine;

namespace GameCore.Battle.Data
{
    [Serializable]
    internal abstract class BaseEffector
    {
        [OdinSerialize] protected FindTargetComponent findTargetComponent;
        [NonSerialized] public Id name;
        protected float radius;
        protected bool isApplied;
        protected SpriteRenderer radiusRenderer;
        private static Sprite circleSprite = Resources.Load<Sprite>("unit-circle");
        
        [field: SerializeField] public int Price { get; private set; }
        protected virtual bool NeedFindOpponent => true;

        public void Init()
        {
            var props = EntiProps.GetProps(name);
            radius = props.GetValue<RadiusGP>();
            radiusRenderer = new GameObject($"{GetType().Name} Radius").AddComponent<SpriteRenderer>();
            radiusRenderer.sprite = circleSprite;
            var gameObject = radiusRenderer.gameObject;
            gameObject.SetActive(false);
            findTargetComponent.Init(gameObject.transform, !NeedFindOpponent);
            OnInit();
        }

        public void DrawRadius(Vector3 position, Color color, int sortingOrder = 0)
        {
            radiusRenderer.gameObject.SetActive(true);
            radiusRenderer.color = color;
            radiusRenderer.sortingOrder = sortingOrder;
            var transform = radiusRenderer.transform;
            transform.position = position;
        }

        public void EndDrawRadius()
        {
            if (!isApplied)
            {
                radiusRenderer.gameObject.SetActive(false);
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