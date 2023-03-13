using System;
using System.Collections.Generic;
using Battle.Data;
using Battle.Data.GameProperty;
using Common.SingleServices;
using GameCore.Common.SingleServices.Windows;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GameCore.Battle.Data.Components
{
    [Serializable]
    internal class HealthComponent
    {
        [SerializeField] private Vector2 scale = new Vector2(1, 1);
        [SerializeField] private Vector2 offset;
        private GameObject gameObject;
        private Transform transform;
        private HealthBar healthBar;
        private float health;
        public static Dictionary<Transform, HealthComponent> ByTransform { get; } = new();
        
        public void Init(string entityName, GameObject gameObject, bool isOpponent)
        {
            this.gameObject = gameObject; 
            transform = gameObject.transform;
            health = EntitiesProperties.ByName[entityName][nameof(HealthGP)].Value;
            healthBar = HealthBar.Create(health, transform, offset, scale, isOpponent);
            ByTransform.Add(transform, this);
        }
        
        public void OnDestroy()
        {
            ByTransform.Remove(gameObject.transform);
        }

        public void Update()
        {
            healthBar.Update();
        }

        public void Kill()
        {
            TakeDamage(health);
        }

        public void TakeDamage(float damage)
        {
            health -= damage;
            healthBar.SetValue(health);
            AnimText.Create($"{damage}", transform.position, fromWorldSpace: true);

            if (health <= 0)
            {
                healthBar.Destroy();
                Object.Destroy(gameObject);
            }
        }
    }
}