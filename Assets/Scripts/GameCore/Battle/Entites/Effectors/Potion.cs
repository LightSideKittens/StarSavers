using System;
using Battle.Data;
using Battle.Data.GameProperty;
using LSCore.Async;
using UnityEngine;
using Attack = GameCore.Battle.Data.Components.AttackComponent;
using Health = GameCore.Battle.Data.Components.HealthComponent;

namespace GameCore.Battle.Data
{
    [Serializable]
    internal class Potion : BaseEffector
    {
        private float duration;
        private float damage;

        protected override void OnInit()
        {
            var props = EntiProps.GetProps(name);
            duration = props.GetValue<HealthGP>();
            damage = props.GetValue<DamageGP>();
        }
        
        protected override void OnApply()
        {
            radiusRenderer.color = new Color(1f, 0.07f, 0.13f, 0.39f);
            Wait.Delay(duration, () =>
            {
                radiusRenderer.gameObject.SetActive(false);
                isApplied = false;
            });
        }

        private void OnTicked()
        {
            foreach (var target in findTargetComponent.FindAll(radius))
            {
                target.Get<Health>().TakeDamage(damage);
            }
        }
    }
}