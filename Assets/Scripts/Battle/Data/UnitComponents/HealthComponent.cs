using System;
using Animatable;
using DG.Tweening;
using UnityEngine;

namespace Battle.Data.Components
{
    [Serializable]
    public class HealthComponent : BaseHealthComponent
    {
        [SerializeField] private Vector2 scale = new Vector2(1, 1);
        [SerializeField] private Vector2 offset;
        [SerializeField] private Transform visualRoot;
        [SerializeField] private Renderer renderer;
        private Material expose;
        private HealthBar healthBar;
        private static readonly int exposure = Shader.PropertyToID("_Exposure");

        public override void Init(Transform transform, bool isOpponent)
        {
            base.Init(transform, isOpponent);
            healthBar = HealthBar.Create(health, transform, offset, scale, isOpponent);
            expose = renderer.material;
        }

        public override void Reset()
        {
            base.Reset();
            healthBar.Reset();
        }

        public override void Update()
        {
            healthBar.Update();
        }

        protected override void OnDamageTaken(float damage)
        {
            visualRoot.DOShakePosition(0.15f, 0.2f, 25);
            expose.SetFloat(exposure, 1.6f);
            expose.DOFloat(1, exposure, 0.5f);
            healthBar.SetValue(health);
            AnimText.Create($"{(int)damage}", transform.position);
        }

        protected override void OnKilled()
        {
            healthBar.Disable();
        }
    }
}