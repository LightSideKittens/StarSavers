using System;
using Battle.Data;
using Battle.Data.GameProperty;
using GameCore.Battle.Data.Components;
using Sirenix.Serialization;
using UnityEngine;
using static GameCore.Battle.RadiusUtils;

namespace GameCore.Battle.Data
{
    [Serializable]
    internal abstract class BaseEffector
    {
        [OdinSerialize] protected FindTargetComponent findTargetComponent;
        [NonSerialized] public string name;
        protected float radius;
        protected bool isApplied;
        protected static SpriteRenderer radiusRenderer;
        private static Sprite circleSprite = Resources.Load<Sprite>("unit-circle");
        
        [field: SerializeField] public int Price { get; private set; }
        protected virtual bool NeedFindOpponent => true;

        public void Init()
        {
            radius = EntitiesProperties.ByName[name][nameof(RadiusGP)].Value;

            if (radiusRenderer == null)
            {
                radiusRenderer = new GameObject("Radius").AddComponent<SpriteRenderer>();
            }
            
            var gameObject = radiusRenderer.gameObject;
            gameObject.SetActive(false);
            findTargetComponent.Init(gameObject, !NeedFindOpponent);
            OnInit();
        }

        public void DrawRadius(Vector3 position, Color color, int sortingOrder = 0)
        {
            radiusRenderer.gameObject.SetActive(true);
            radiusRenderer.sprite = circleSprite;
            radiusRenderer.color = color;
            radiusRenderer.sortingOrder = sortingOrder;
            var transform = radiusRenderer.transform;
            transform.position = position;
            transform.localScale = new Vector2(X * radius, Y * radius) * 2;
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