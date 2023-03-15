using System;
using Battle.Data;
using Battle.Data.GameProperty;
using BeatRoyale;
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
        private string attackSpeed;
        private int currentIndex;
        private TactListener listener;

        protected override void OnInit()
        {
            var properties = EntitiesProperties.ByName[name];
            duration = properties[nameof(HealthGP)].Value;
            attackSpeed = Convert.ToString((int)properties[nameof(AttackSpeedGP)].value, 2);
            damage = properties[nameof(DamageGP)].Value;
            listener = TactListener.Listen(-0.1f);
        }
        
        protected override void OnApply()
        {
            radiusRenderer.color = new Color(1f, 0.07f, 0.13f, 0.39f);
            var timer = new CountDownTimer(duration);
            listener.Ticked += OnTicked;

            timer.Stopped += () =>
            {
                listener.Ticked -= OnTicked;
                radiusRenderer.gameObject.SetActive(false);
                isApplied = false;
            };
        }

        private void OnTicked()
        {
            var step = attackSpeed[currentIndex % attackSpeed.Length];

            if (step == '1')
            {
                foreach (var target in findTargetComponent.FindAll(radius))
                {
                    target.Get<Health>().TakeDamage(damage);
                }
            }

            currentIndex++;
        }
    }
}