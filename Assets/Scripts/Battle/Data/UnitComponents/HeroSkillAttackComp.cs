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
        public float joystickMagnitudeThreshold = 1;
        private bool isCancelled;
        private bool isAim;
        protected virtual ProgressJoystick Joystick => BattleWindow.SkillJoystick;
        public void SetActivePreview(bool active) => impactObject.SetActivePreview(active);
        public void LookAt(in Vector2 direction) => impactObject.LookAt(direction);

        protected override void Init()
        {
            base.Init();
            joystick = Joystick;
            joystick.IsUsing.Changed += SetIsRunning;
            joystick.IsUsing.Changed += OnJoystickUsing;
            joystick.Magnitude.Changed += OnJoystickMagnitudeChanged;
            joystick.Progress = 1;
            impactObject.SetActivePreview(false);
        }

        private void OnJoystickMagnitudeChanged(float value)
        {
            bool isThreshold = value > joystickMagnitudeThreshold;
            impactObject.SetActivePreview(isThreshold);
            
            if (isThreshold)
            {
                isAim = true;
                isCancelled = false;
            }
            else if(isAim)
            {
                isCancelled = true;
            }
        }

        private void OnJoystickUsing(bool isUsing)
        {
            if (!isUsing)
            {
                impactObject.SetActivePreview(false);
                if (!isCancelled)
                {
                    if (!isAim)
                    {
                        if (findTargetComp.Find(out var target))
                        {
                            impactObject.LookAt(target);
                        }
                    }
                
                    Attack();
                }
            }

            isAim = false;
            isCancelled = false;
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