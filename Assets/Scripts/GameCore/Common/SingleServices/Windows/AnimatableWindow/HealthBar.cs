using System;
using LSCore;
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
        private Camera camera;
        private Vector3 offset;

        public void Reset()
        {
            slider.gameObject.SetActive(true);
            slider.value = slider.maxValue;
        }
        
        public static HealthBar Create(float maxValue, Transform target, Vector2 offset, Vector2 scale, bool isOpponent)
        {
            var spawnPoint = SpawnPoint;
            var template = isOpponent ? OpponentHealthBar : AnimatableWindow.HealthBar;
            Vector2 position = target.position;
            position += offset;
            var camera = Camera.main;
            position = camera.WorldToScreenPoint(position);

            var slider = Object.Instantiate(template.slider, position, Quaternion.identity);
            var sliderTransform = slider.transform;
            sliderTransform.position = position;
            slider.transform.SetParent(spawnPoint.parent, false);
            sliderTransform.localScale = scale;
            slider.maxValue = maxValue;
            slider.value = maxValue;

            return new HealthBar
            {
                camera = camera,
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
            var targetLocalPosByCam = camera.transform.InverseTransformPoint(target.position + offset);
            targetLocalPosByCam /= BaseWindow<AnimatableWindow>.Canvas.transform.lossyScale.x;
            targetLocalPosByCam.z = 0;
            slider.transform.localPosition = targetLocalPosByCam;
        }

        public void SetValue(float value)
        {
            slider.value = value;
        }
    }
}