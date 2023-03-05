using System;
using Core.Extensions.Unity;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace BeatRoyale.Windows
{
    internal partial class ControlPanel
    {
        [Serializable] 
        private class Tab
        {
            private static Tab lastActiveTab;
            [SerializeField] private Button button;

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
                
                layoutElement = button.GetComponent<LayoutElement>();
                transform = button.transform as RectTransform;

                defaultWidth = layoutElement.preferredWidth;
                targetWidth = defaultWidth + animationData.widthOffset;
                
                defaultHeight = transform.sizeDelta.y;
                targetHeight = defaultHeight + animationData.heightOffset;
                button.AddListener(OnTabClicked);
            }

            public void SetActive()
            {
                lastActiveTab = this;
                Select();
            }
            
            private void OnTabClicked()
            {
                if (lastActiveTab != this)
                {
                    Select();
                    lastActiveTab.Deselect();
                    lastActiveTab = this;
                }
            }

            private void Select()
            {
                showWindow();
                button.interactable = false;
                DOTween.To(value => { layoutElement.preferredWidth = value;}, layoutElement.preferredWidth, targetWidth, duration).SetEase(Ease.InOutCubic);
                DOTween.To(value =>
                {
                    var size = transform.sizeDelta;
                    size.y = value;
                    transform.sizeDelta = size;
                }, transform.sizeDelta.y, targetHeight, duration).SetEase(Ease.InOutCubic).OnComplete(() => button.interactable = true);
            }

            private void Deselect()
            {
                hideWindow();
                button.interactable = false;
                DOTween.To(value => { layoutElement.preferredWidth = value;}, layoutElement.preferredWidth, defaultWidth, duration).SetEase(Ease.InOutCubic);
                DOTween.To(value =>
                {
                    var size = transform.sizeDelta;
                    size.y = value;
                    transform.sizeDelta = size;
                }, transform.sizeDelta.y, defaultHeight, duration).SetEase(Ease.InOutCubic).OnComplete(() => button.interactable = true);;
            }
        }
    }
}