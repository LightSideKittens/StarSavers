using DG.Tweening;
using LSCore;
using UnityEngine;

namespace BeatHeroes.Windows
{
    public class BaseLauncherWindow<T> : BaseWindow<T> where T : BaseLauncherWindow<T>
    {
        public static int Index => Instance.Internal_Index;
        protected virtual int Internal_Index { get; }
        private float XShow => MainWindow.CurrentShowedWindowIndex > Internal_Index ? 1 : -1;
        private float XHide => MainWindow.CurrentShowingWindowIndex > Internal_Index ? 1 : -1;
        protected override float DefaultAlpha => 1;
        private Vector2 startPivot;

        protected override void Init()
        {
            base.Init();
            startPivot = RectTransform.pivot;
        }

        protected override void OnShowing()
        {
            base.OnShowing();
            MainWindow.CurrentShowingWindowIndex = Internal_Index;
        }
        
        protected override void OnShowed()
        {
            base.OnShowed();
            MainWindow.CurrentShowedWindowIndex = Internal_Index;
        }

        protected override Tween ShowAnim
        {
            get
            {
                var pivot = startPivot;
                pivot.x += XShow;
                RectTransform.pivot = pivot;
                var sequence = DOTween.Sequence()
                    .Insert(0, RectTransform.DOPivotX(startPivot.x, fadeSpeed));
                return sequence;
            }
        }

        protected override Tween HideAnim
        {
            get
            {
                var sequence = DOTween.Sequence()
                    .Insert(0, RectTransform.DOPivotX(startPivot.x + XHide, fadeSpeed));
            
                return sequence;
            }
        }
    }
}