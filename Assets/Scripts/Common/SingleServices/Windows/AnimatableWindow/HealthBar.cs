using System;
using UnityEngine;
using UnityEngine.UI;
using static Animatable.AnimatableWindow;
using Object = UnityEngine.Object;

namespace Animatable
{
    [Serializable]
    public class HealthBar
    {
        [SerializeField] public Slider slider;
        private Transform target;
        private Vector3 offset;

        public void Reset()
        {
            slider.gameObject.SetActive(true);
            slider.value = slider.maxValue;
        }
        
        public static HealthBar Create(float maxValue, Transform target, Vector2 offset, Vector2 scale, bool isOpponent)
        {
            var template = isOpponent ? OpponentHealthBar : AnimatableWindow.HealthBar;
            
            var slider = Object.Instantiate(template.slider);
            var sliderTransform = slider.transform;
            sliderTransform.SetParent(SpawnPoint, false);
            sliderTransform.localScale = scale;
            slider.maxValue = maxValue;
            slider.value = maxValue;

            return new HealthBar
            {
                target = target,
                offset = offset,
                slider = slider,
            };
        }

        public void Disable()
        { 
            slider.gameObject.SetActive(false);
        }

        public void Update()
        {
            slider.transform.localPosition = GetLocalPosition(target.position + offset);
        }

        public void SetValue(float value)
        {
            slider.value = value;
        }
    }
}