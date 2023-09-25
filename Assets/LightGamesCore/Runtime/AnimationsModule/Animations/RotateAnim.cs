using System;
using DG.Tweening;
using UnityEngine;

namespace LGCore.AnimationsModule.Animations
{
    [Serializable]
    public class RotateAnim : BaseAnim<Vector3>
    {
        public Transform target;

        protected override void Internal_Init()
        {
            target.eulerAngles = startValue;
        }
        
        protected override Tween Internal_Animate()
        {
            return target.DORotate(endValue, duration);
        }
    }
}