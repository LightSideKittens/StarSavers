﻿using DG.Tweening;
using LSCore;
using LSCore.AnimationsModule.Animations;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace MultiWars
{
    public class Welcomer : ServiceManager<Welcomer>
    {
        [SerializeField] private SliderAnim sliderAnim;
        [SerializeField] private SliderAnim completedSliderAnim;
        private Tween tween;
        
        protected override void Awake()
        {
            base.Awake();
            tween = sliderAnim.Animate();
            Addressables.LoadSceneAsync("Launcher").OnSuccess(OnSuccess).OnError(OnError);
            
            return;

            void OnSuccess()
            {
                tween.Kill();
                completedSliderAnim.Animate();
            }

            void OnError()
            {
                tween.Kill();
            }
        }
    }
}