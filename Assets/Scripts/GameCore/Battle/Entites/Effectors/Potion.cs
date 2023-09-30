using System;
using Battle.Data;
using Battle.Data.GameProperty;
using BeatRoyale;
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
            var properties = EntiProps.ByName[name];
            duration = properties[nameof(HealthGP)].Value;
            damage = properties[nameof(DamageGP)].Value;
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