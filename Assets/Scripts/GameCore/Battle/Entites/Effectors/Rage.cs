using System;
using Battle.Data;
using Battle.Data.GameProperty;
using GameCore.Battle.Data.Components;
using UnityEngine;

namespace GameCore.Battle.Data
{
    [Serializable]
    internal class Rage : BaseEffector
    {
        private float duration;
        private float damageBuff;
        private float moveSpeedBuff;
        protected override bool NeedFindOpponent => false;

        protected override void OnInit()
        {
            var properties = EntitiesProperties.ByName[name];
            duration = properties[nameof(HealthGP)].Value;
            damageBuff = properties[nameof(DamageGP)].Value / 100f;
            moveSpeedBuff = properties[nameof(MoveSpeedGP)].Value / 100f;
        }

        protected override void OnApply()
        {
            radiusRenderer.color = new Color(0.68f, 0.17f, 1f, 0.39f);
            var timer = new CountDownTimer(duration, true);
            timer.Updated += x =>
            {
                foreach (var target in findTargetComponent.FindAll(radius))
                {
                    MoveComponent.ByTransform[target].Buffs.Set(nameof(Rage), moveSpeedBuff, 0.1f);
                }
            };

            timer.Stopped += () =>
            {
                radiusRenderer.gameObject.SetActive(false);
                isApplied = false;
            };
        }
    }
}