using System;
using UnityEngine;

namespace BeatRoyale.Launcher
{
    public partial class LauncherWorld
    {
        [Serializable]
        private struct BackgroundAnimationData
        {
            [SerializeField, Range(0, 0.5f)] private float backgroundSpeed;
            [SerializeField] private MeshRenderer background;

            public void Update()
            {
                background.material.mainTextureOffset += Vector2.up * (Time.deltaTime * backgroundSpeed);
            }
        }
    }
}