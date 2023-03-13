using System.Collections.Generic;
using Battle.Data;
using Battle.Data.GameProperty;
using GameCore.Battle.Data.Components;
using UnityEngine;

namespace GameCore.Battle.Data
{
    internal class Rage : BaseEffector
    {
        private float duration;
        private int damageBuff;
        private int moveSpeedBuff;
        private List<Transform> toRemove = new();

        protected override void OnInit()
        {
            var properties = EntitiesProperties.ByName[name];
            duration = properties[nameof(HealthGP)].Value;
            damageBuff = (int)properties[nameof(DamageGP)].Value;
            moveSpeedBuff = (int)properties[nameof(MoveSpeedGP)].Value;
        }

        protected override void OnApply()
        {
            radiusRenderer.color = new Color(0.68f, 0.17f, 1f, 0.39f);
            new CountDownTimer(duration, true).Updated += x =>
            {
                toRemove.Clear();
                foreach (var target in findTargetComponent.FindAll(lastPosition, radius))
                {
                    MoveComponent.ByTransform[target].IncreaseSpeedByPercent(moveSpeedBuff);
                }
            };
        }
    }
}