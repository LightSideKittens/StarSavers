using System;
using DG.Tweening;
using LSCore;
using TMPro;
using UnityEngine;
using static Animatable.AnimatableWindow;

namespace Animatable
{
    [Serializable]
    public class AnimText
    {
        [SerializeField] private TMP_Text text;
        [SerializeField] private Vector2 animOffset = new Vector2(0, 100);
        [SerializeField] private float duration = 1;
        private OnOffPool<TMP_Text> pool;

        internal void Init() => pool = new OnOffPool<TMP_Text>(text);

        public static AnimText Create(string message, Vector2 pos = default, Vector2 offset = default, bool fromWorldSpace = false)
        {
            var template = AnimatableWindow.AnimText;

            if (fromWorldSpace)
            {
                pos = GetLocalPosition(pos + offset);
            }

            var scale = template.text.transform.localScale;
            var text = template.pool.Get();
            var textTransfrom = text.transform;
            textTransfrom.SetParent(SpawnPoint, true);
            textTransfrom.localPosition = pos;
            textTransfrom.localScale = scale;
            text.text = message;
            var rect = (RectTransform) text.transform;
            rect.DOAnchorPos(rect.anchoredPosition + template.animOffset, template.duration);
            text.alpha = 1;
            text.DOFade(0, template.duration).OnComplete(() =>
            {
                template.pool.Release(text);
            });

            return new AnimText
            {
                text = text
            };
        }
    }
}