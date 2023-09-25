using System;
using Battle.Data;
using Battle.Data.GameProperty;
using DG.Tweening;
using LGCore.Async;
using UnityEngine;
using Move = GameCore.Battle.Data.Components.MoveComponent;
using Attack = GameCore.Battle.Data.Components.AttackComponent;

namespace GameCore.Battle.Data
{
    [Serializable]
    internal class Rage : BaseEffector
    {
        private const string Name = nameof(Rage);
        
        private float duration;
        private float damageBuff;
        private float moveSpeedBuff;
        protected override bool NeedFindOpponent => false;
        [SerializeField] private float buffDuration = 0.1f;

        protected override void OnInit()
        {
            var properties = EntiProps.ByName[name];
            duration = properties[nameof(HealthGP)].Value;
            damageBuff = properties[nameof(DamageGP)].Value / 100;
            moveSpeedBuff = properties[nameof(MoveSpeedGP)].Value / 100;
        }

        protected override void OnApply()
        {
            radiusRenderer.color = new Color(0.68f, 0.17f, 1f, 0.39f);

            Wait.Run(duration, _ =>
            {
                foreach (var target in findTargetComponent.FindAll(radius))
                {
                    target.Get<Move>().Buffs.Set(Name, moveSpeedBuff, buffDuration);
                    target.Get<Attack>().Buffs.Set(Name, damageBuff, buffDuration);
                }
            }).OnComplete(() =>
            {
                radiusRenderer.gameObject.SetActive(false);
                isApplied = false;
            });
        }
    }
}