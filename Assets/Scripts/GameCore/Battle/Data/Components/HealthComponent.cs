using System;
using Animatable;
using Battle;
using Battle.Data.GameProperty;
using UnityEngine;
using static GameCore.Battle.ObjectsByTransfroms<GameCore.Battle.Data.Components.HealthComponent>;

namespace GameCore.Battle.Data.Components
{
    [Serializable]
    internal class HealthComponent
    {
        [SerializeField] private Vector2 scale = new Vector2(1, 1);
        [SerializeField] private Vector2 offset;
        private Transform transform;
        private HealthBar healthBar;
        private bool isOpponent;
        private float health;

        public void Init(Transform transform, bool isOpponent)
        {
            this.transform = transform;
            health = transform.GetValue<HealthGP>();
            healthBar = HealthBar.Create(health, transform, offset, scale, isOpponent);
            this.isOpponent = isOpponent;
            Add(transform, this);
        }

        public void Reset() => healthBar.Reset();

        public void Destroy()
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
                healthBar.Disable();
                
                if (isOpponent)
                {
                    OpponentWorld.Pool.Release(transform);
                }
                else
                {
                    transform.Get<Unit>().Destroy();
                }
            }
        }
    }
}