using DG.Tweening;
using UnityEngine;

namespace BeatRoyale.Windows
{
    public class BaseLauncherWindow<T> : BaseWindow<T> where T : BaseLauncherWindow<T>
    {
        public static int Index => Instance.Internal_Index;
        
        protected override Transform Parent => ControlPanel.Instance.transform;
        protected virtual int Internal_Index { get; }
        private float XShow => ControlPanel.CurrentShowedWindowIndex > Internal_Index ? ControlPanel.LeftX : ControlPanel.RightX;
        private float XHide => ControlPanel.CurrentShowingWindowIndex > Internal_Index ? ControlPanel.LeftX : ControlPanel.RightX;
        public override float DefaultAlpha => 1;

        protected override void Init()
        {
            base.Init();
            var center = new Vector2(0.5f, 0.5f);
            RectTransform.anchorMin = center;
            RectTransform.anchorMax = center;
            RectTransform.sizeDelta = ControlPanel.Rect.size;
        }

        protected override void OnShowing()
        {
            base.OnShowing();
            ControlPanel.CurrentShowingWindowIndex = Internal_Index;
        }
        
        protected override void OnShowed()
        {
            base.OnShowed();
            ControlPanel.CurrentShowedWindowIndex = Internal_Index;
        }

        protected override Tween GetShowAnimation()
        {
            RectTransform.anchoredPosition = new Vector2(XShow, 0);
            var sequence = DOTween.Sequence()
                /*.Insert(0, base.GetShowAnimation())*/
                .Insert(0, RectTransform.DOAnchorPos(Vector2.zero, fadeSpeed));
            return sequence;
        }
        
        protected override Tween GetHideAnimation()
        {
            var sequence = DOTween.Sequence()
                /*.Insert(0, base.GetHideAnimation())*/
                .Insert(0, RectTransform.DOAnchorPos(new Vector2(XHide, 0), fadeSpeed));
            
            return sequence;
        }
    }
}