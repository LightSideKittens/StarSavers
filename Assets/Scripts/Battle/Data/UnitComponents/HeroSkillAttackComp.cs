using System;
using Battle.Windows;
using DG.Tweening;
using LSCore.Async;
using UnityEngine;
using UnityEngine.Scripting;

namespace LSCore.BattleModule
{
    [Preserve, Serializable]
    public class HeroSkillAttackComp : BaseAttackComponent
    {
        private bool canAttack = true;
        private ProgressJoystick joystick;
        protected virtual ProgressJoystick Joystick => BattleWindow.SkillJoystick;
        public void SetActivePreview(bool active) => impactObject.SetActivePreview(active);
        public void LookAt(in Vector3 direction) => impactObject.LookAt(direction);

        protected override void Init()
        {
            base.Init();
            joystick = Joystick;
            joystick.IsUsing.Changed += SetIsRunning;
            joystick.IsUsing.Changed += OnJoystickUsing;
            joystick.Progress = 1;
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
                Wait.Run(attackSpeed, SetProgress).OnComplete(AllowAttack);
                impactObject.Emit();
            }
        }

        private void SetProgress(float value)
        {
            joystick.Progress = value;
        }

        private void AllowAttack() => canAttack = true;
    }
}