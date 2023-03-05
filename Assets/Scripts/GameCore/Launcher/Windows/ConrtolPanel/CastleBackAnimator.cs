using System;
using DG.Tweening;
using UnityEngine;

namespace BeatRoyale.Windows
{
    [Serializable]
    internal class CastleBackAnimator
    {
        private static readonly int directionalDistortionInvert = Shader.PropertyToID("_DirectionalDistortionInvert");
        private static readonly int directionalDistortionFade = Shader.PropertyToID("_DirectionalDistortionFade");
        
        [SerializeField, Range(0, 0.5f)] private float backgroundSpeed;
        [SerializeField] private MeshRenderer background;
        [SerializeField] private float amplitude;
        [SerializeField] private Material castleMaterial;
        [SerializeField] private SpriteRenderer castleBack;
        [SerializeField] private SpriteRenderer castle;
        [SerializeField] private float speed = 0.2f;
        private Material instanceCastleMaterial;
        private float time;

        public void Init()
        {
            instanceCastleMaterial = new Material(castleMaterial);
            castleBack.material = instanceCastleMaterial;
            castle.material = instanceCastleMaterial;
            MatchWindow.Showing += OnShowing;
            MatchWindow.Hiding += OnHiding;
        }

        public void Update()
        {
            time += Time.deltaTime;
            castleBack.transform.position -= new Vector3(0, Mathf.Sin(time)) * amplitude;
            background.material.mainTextureOffset += Vector2.up * (Time.deltaTime * backgroundSpeed);
        }

        private void OnShowing()
        {
            var index = ControlPanel.CurrentShowedWindowIndex - MatchWindow.Index;

            if (index > 0)
            {
                ShowLeft();
            }
            else
            {
                ShowRight();
            }
        }
        
        private void OnHiding()
        {
            var index = ControlPanel.CurrentShowingWindowIndex - MatchWindow.Index;

            if (index > 0)
            {
                HideLeft();
            }
            else
            {
                HideRight();
            }
        }
        
        private void HideLeft()
        {
            instanceCastleMaterial.SetFloat(directionalDistortionInvert, 1);
            instanceCastleMaterial.SetFloat(directionalDistortionFade, -16);
            instanceCastleMaterial.DOFloat(16, directionalDistortionFade, speed);
        }
        
        private void HideRight()
        {
            instanceCastleMaterial.SetFloat(directionalDistortionInvert, 0);
            instanceCastleMaterial.SetFloat(directionalDistortionFade, 16);
            instanceCastleMaterial.DOFloat(-16, directionalDistortionFade, speed);
        }
        
        private void ShowLeft()
        {
            instanceCastleMaterial.SetFloat(directionalDistortionInvert, 1);
            instanceCastleMaterial.SetFloat(directionalDistortionFade, 16);
            instanceCastleMaterial.DOFloat(-16, directionalDistortionFade, speed);
        }
        
        private void ShowRight()
        {
            instanceCastleMaterial.SetFloat(directionalDistortionInvert, 0);
            instanceCastleMaterial.SetFloat(directionalDistortionFade, -16);
            instanceCastleMaterial.DOFloat(16, directionalDistortionFade, speed);
        }
        
    }
}