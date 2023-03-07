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
        private Material defaultMaterial;
        private Material effectMaterial;
        private float time;

        public void Init()
        {
            /*defaultMaterial = castleBack.material;
            effectMaterial = new Material(castleMaterial);*/
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
            castleBack.gameObject.SetActive(true);
            castle.gameObject.SetActive(true);
            /*castleBack.material = effectMaterial;
            castle.material = effectMaterial;*/
            
            var index = ControlPanel.CurrentShowedWindowIndex - MatchWindow.Index;

            if (index > 0)
            {
                ShowLeft(OnShowComplete);
            }
            else
            {
                ShowRight(OnShowComplete);
            }
            
            void OnShowComplete()
            {
                background.transform.parent.gameObject.SetActive(false);
                /*castleBack.material = defaultMaterial;
                castle.material = defaultMaterial;*/
            }
        }
        
        private void OnHiding()
        {
            background.transform.parent.gameObject.SetActive(true);
            /*castleBack.material = effectMaterial;
            castle.material = effectMaterial;*/
            var index = ControlPanel.CurrentShowingWindowIndex - MatchWindow.Index;

            if (index > 0)
            {
                HideLeft(OnHideComplete);
            }
            else
            {
                HideRight(OnHideComplete);
            }

            void OnHideComplete()
            {
                castleBack.gameObject.SetActive(false);
                castle.gameObject.SetActive(false);
            }
        }
        
        private void HideLeft(TweenCallback onComplete)
        {
            /*effectMaterial.SetFloat(directionalDistortionInvert, 1);
            effectMaterial.SetFloat(directionalDistortionFade, -16);
            effectMaterial.DOFloat(16, directionalDistortionFade, speed).OnComplete(onComplete);*/
            castleBack.DOFade(0, speed).OnComplete(onComplete);
            castle.DOFade(0, speed).OnComplete(onComplete);
        }
        
        private void HideRight(TweenCallback onComplete)
        {
            /*effectMaterial.SetFloat(directionalDistortionInvert, 0);
            effectMaterial.SetFloat(directionalDistortionFade, 16);
            effectMaterial.DOFloat(-16, directionalDistortionFade, speed).OnComplete(onComplete);*/
            castleBack.DOFade(0, speed).OnComplete(onComplete);
            castle.DOFade(0, speed).OnComplete(onComplete);
        }
        
        private void ShowLeft(TweenCallback onComplete)
        {
            /*effectMaterial.SetFloat(directionalDistortionInvert, 1);
            effectMaterial.SetFloat(directionalDistortionFade, 16);
            effectMaterial.DOFloat(-16, directionalDistortionFade, speed).OnComplete(onComplete);*/
            castleBack.DOFade(1, speed).OnComplete(onComplete);
            castle.DOFade(1, speed).OnComplete(onComplete);
        }
        
        private void ShowRight(TweenCallback onComplete)
        {
            /*effectMaterial.SetFloat(directionalDistortionInvert, 0);
            effectMaterial.SetFloat(directionalDistortionFade, -16);
            effectMaterial.DOFloat(16, directionalDistortionFade, speed).OnComplete(onComplete);*/
            castleBack.DOFade(1, speed).OnComplete(onComplete);
            castle.DOFade(1, speed).OnComplete(onComplete);
        }
        
    }
}