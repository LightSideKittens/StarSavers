using System;
using BeatRoyale.Windows;
using Core.SingleService;
using GameCore.Attributes;
using UnityEngine;

namespace BeatRoyale.Launcher
{
    public class LauncherBootstrap : ServiceManager
    {
        [Serializable]
        private struct BackgroundAnimationData
        {
            [SerializeField, Range(0, 0.5f)] private float backgroundSpeed;
            [SerializeField] private Material background;

            public void Update()
            {
                background.mainTextureOffset += Vector2.up * (Time.deltaTime * backgroundSpeed);
            }
        }

        [ColoredField, SerializeField] private BackgroundAnimationData backgroundData;
        
        protected override void Awake()
        {
            base.Awake();
            Application.targetFrameRate = 60;
            ControlPanel.Show();
        }

        private void Update()
        {
            backgroundData.Update();
        }
    }
}

