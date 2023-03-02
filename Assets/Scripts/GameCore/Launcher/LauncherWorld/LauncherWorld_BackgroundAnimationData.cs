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
            [SerializeField] private SpriteRenderer castelBack;
            [SerializeField] private float amplitude;
            private float time;

            public void Update()
            {
                time += Time.deltaTime;
                castelBack.transform.position -= new Vector3(0, Mathf.Sin(time)) * amplitude;
                background.material.mainTextureOffset += Vector2.up * (Time.deltaTime * backgroundSpeed);
            }
        }
    }
}