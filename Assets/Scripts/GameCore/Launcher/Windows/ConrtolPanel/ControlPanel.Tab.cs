using System;
using Core.Extensions.Unity;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace BeatRoyale.Windows
{
    public partial class ControlPanel
    {
        [Serializable] 
        private class Tab
        {
            [SerializeField] private Toggle toggle;

            private LayoutElement layoutElement;
            private RectTransform transform;
            
            private float duration;
            
            private float targetWidth;
            private float defaultWidth;
            
            private float targetHeight;
            private float defaultHeight;

            private Action showWindow;
            private Action hideWindow;

            public static TabAnimationData animationData;

            public void Init<T>() where T : BaseWindow<T>
            {
                duration = animationData.duration;
                showWindow = BaseWindow<T>.Show;
                hideWindow = BaseWindow<T>.Hide;
                
                layoutElement = toggle.GetComponent<LayoutElement>();
                transform = toggle.transform as RectTransform;

                defaultWidth = layoutElement.preferredWidth;
                targetWidth = defaultWidth + animationData.widthOffset;
                
                defaultHeight = transform.sizeDelta.y;
                targetHeight = defaultHeight + animationData.heightOffset;
                toggle.AddListener(OnToggleValueChanged);

                if (toggle.isOn)
                {
                    Select();
                }
            }
            
            private void OnToggleValueChanged(bool isActive)
            {
                if (isActive)
                {
                    showWindow?.Invoke();
                    Select();
                }
                else
                {
                    hideWindow?.Invoke();
                    Deselect();
                }
            }

            private void Select()
            {
                DOTween.To(value => { layoutElement.preferredWidth = value;}, layoutElement.preferredWidth, targetWidth, duration).SetEase(Ease.InOutCubic);
                var endValue = transform.sizeDelta;
                endValue.y = targetHeight;
                transform.DOSizeDelta(endValue, duration).SetEase(Ease.InOutCubic);;
            }

            private void Deselect()
            {
                DOTween.To(value => { layoutElement.preferredWidth = value;}, layoutElement.preferredWidth, defaultWidth, duration).SetEase(Ease.InOutCubic);
                var endValue = transform.sizeDelta;
                endValue.y = defaultHeight;
                transform.DOSizeDelta(endValue, duration).SetEase(Ease.InOutCubic);
            }
        }
    }
}