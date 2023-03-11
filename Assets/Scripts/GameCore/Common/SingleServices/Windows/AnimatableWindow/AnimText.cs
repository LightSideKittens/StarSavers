using System;
using Common.SingleServices.Windows;
using Core.ReferenceFrom.Extensions.Unity;
using DG.Tweening;
using TMPro;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Common.SingleServices
{
    [Serializable]
    public class AnimText
    {
        [SerializeField] private GameObject gameObject;
        [ReferenceFrom(nameof(gameObject))] [SerializeField] private TMP_Text text;
        [SerializeField] private Vector2 animOffset = new Vector2(0, 100);
        [SerializeField] private float duration = 1;

        public static AnimText Create(string message, Vector2 pos = default, Vector2 offset = default, bool fromWorldSpace = false)
        {
            var spawnPoint = AnimatableWindow.SpawnPoint;
            var animText = AnimatableWindow.AnimText;
            var position = pos == default ? (Vector2)spawnPoint.position : pos;
            position += offset;

            if (fromWorldSpace)
            {
                var camera = Camera.main;
                position = camera.WorldToScreenPoint(pos);
            }

            var scale = animText.gameObject.transform.localScale;
            var obj = Object.Instantiate(animText.gameObject, position, Quaternion.identity);
            obj.transform.SetParent(spawnPoint.parent, true);
            obj.transform.localScale = scale;
            var text = obj.Get(animText.text);
            text.text = message;
            var rect = (RectTransform) obj.transform;
            rect.DOAnchorPos(rect.anchoredPosition + animText.animOffset, animText.duration);
            text.DOFade(0, animText.duration).OnComplete(() => {Object.Destroy(obj);});

            return new AnimText
            {
                gameObject = obj,
                text = text,
            };
        }
    }
}