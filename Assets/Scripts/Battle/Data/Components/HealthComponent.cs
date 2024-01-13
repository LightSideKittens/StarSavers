using System;
using Animatable;
using DG.Tweening;
using LSCore.LevelSystem;
using UnityEngine;
using static Battle.ObjectsByTransfroms<Battle.Data.Components.HealthComponent>;

namespace Battle.Data.Components
{
    [Serializable]
    internal class HealthComponent
    {
        [SerializeField] private Vector2 scale = new Vector2(1, 1);
        [SerializeField] private Vector2 offset;
        [SerializeField] private Transform visualRoot;
        [SerializeField] private Renderer renderer;

        private Material expose;
        private Transform transform;
        private HealthBar healthBar;
        private bool isKilled;
        private bool isOpponent;
        private float health;
        private static readonly int exposure = Shader.PropertyToID("_Exposure");

        public void Init(Transform transform, bool isOpponent)
        {
            this.transform = transform;
            health = transform.GetValue<HealthGP>();
            healthBar = HealthBar.Create(health, transform, offset, scale, isOpponent);
            this.isOpponent = isOpponent;
            expose = renderer.material;
            Add(transform, this);
        }
        
        public void Reset()
        {
            isKilled = false;
            health = transform.GetValue<HealthGP>();
            healthBar.Reset();
        }

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
            if (isKilled) return;
            
            health -= damage;
            visualRoot.DOShakePosition(0.15f, 0.2f, 25);
            expose.SetFloat(exposure, 1.6f);
            expose.DOFloat(1, exposure, 0.5f);
            healthBar.SetValue(health);
            AnimText.Create($"{(int)damage}", transform.position);

            if (health <= 0)
            {
                isKilled = true;
                healthBar.Disable();
                transform.Get<Unit>().Kill();
            }
        }
    }
}