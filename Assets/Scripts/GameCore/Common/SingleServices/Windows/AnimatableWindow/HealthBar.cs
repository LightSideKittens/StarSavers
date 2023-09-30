using System;
using Common.SingleServices.Windows;
using LSCore.Extensions.Unity;
using UnityEngine;
using UnityEngine.UI;
using static Common.SingleServices.Windows.AnimatableWindow;
using Object = UnityEngine.Object;

namespace GameCore.Common.SingleServices.Windows
{
    [Serializable]
    public class HealthBar
    {
        [SerializeField] private Slider slider;
        private Transform target;
        private Camera camera;
        private Vector3 offset;

        public static HealthBar Create(float maxValue, Transform target, Vector2 offset, Vector2 scale, bool isOpponent)
        {
            var spawnPoint = SpawnPoint;
            var healthBar = isOpponent ? OpponentHealthBar : AnimatableWindow.HealthBar;
            Vector2 position = target.position;
            position += offset;
            var camera = Camera.main;
            position = camera.WorldToScreenPoint(position);

            var slider = Object.Instantiate(healthBar.slider, position, Quaternion.identity);
            slider.transform.SetParent(spawnPoint.parent, false);
            slider.transform.localScale = scale;
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

        public void Destroy()
        {
            Object.Destroy(slider.gameObject);
        }

        public void Update()
        {
            var screenPoint = camera.transform.InverseTransformPoint(target.position + offset);
            screenPoint /= AnimatableWindow.Canvas.transform.lossyScale.x;
            screenPoint.z = 0;
            slider.transform.localPosition = screenPoint;
        }

        public void SetValue(float value)
        {
            slider.value = value;
        }
    }
}