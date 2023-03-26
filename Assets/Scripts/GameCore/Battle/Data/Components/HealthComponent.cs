using System;
using Battle.Data.GameProperty;
using Common.SingleServices;
using GameCore.Common.SingleServices.Windows;
using UnityEngine;
using static GameCore.Battle.ObjectsByTransfroms<GameCore.Battle.Data.Components.HealthComponent>;
using Object = UnityEngine.Object;

namespace GameCore.Battle.Data.Components
{
    [Serializable]
    internal class HealthComponent
    {
        [SerializeField] private Vector2 scale = new Vector2(1, 1);
        [SerializeField] private Vector2 offset;
        private Transform transform;
        private HealthBar healthBar;
        private float health;

        public void Init(Transform transform, bool isOpponent)
        {
            this.transform = transform;
            health = BaseEntity.GetProperties(transform)[nameof(HealthGP)].Value;
            healthBar = HealthBar.Create(health, transform, offset, scale, isOpponent);
            Add(transform, this);
        }
        
        public void OnDestroy()
        { 
            Remove(transform);
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
            AnimText.Create($"{(int)damage}", transform.position, fromWorldSpace: true);

            if (health <= 0)
            {
                healthBar.Destroy();
                Object.Destroy(transform.gameObject);
            }
        }
    }
}