using System;
using Battle.Windows;
using LSCore.Async;
using UnityEngine;
using UnityEngine.Scripting;

namespace LSCore.BattleModule
{
    [Preserve, Serializable]
    public class HeroSkillAttackComp : BaseAttackComponent
    {
        private bool canAttack = true;
        private Joystick joystick;
        public void SetActivePreview(bool active) => impactObject.SetActivePreview(active);
        public void LookAt(in Vector3 direction) => impactObject.LookAt(direction);

        protected override void Init()
        {
            base.Init();
            joystick = BattleWindow.SkillJoystick;
            joystick.IsUsing.Changed += SetIsRunning;
            joystick.IsUsing.Changed += OnJoystickUsing;
            impactObject.SetActivePreview(false);
        }

        private void OnJoystickUsing(bool isUsing)
        {
            impactObject.SetActivePreview(isUsing);
            if (!isUsing)
            {
                Attack();
            }
        }

        protected override void Update()
        {
            base.Update();
            impactObject.LookAt(joystick.Direction);
        }

        public void Attack()
        {
            if (canAttack)
            {
                canAttack = false;
                Wait.Delay(attackSpeed, AllowAttack);
                impactObject.Emit();
            }
        }

        private void AllowAttack() => canAttack = true;
    }
}