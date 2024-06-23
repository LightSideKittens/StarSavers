using DG.Tweening;
using LSCore;
using LSCore.Extensions.Unity;
using UnityEngine;

namespace MultiWars.Windows
{
    public struct LauncherWindowsData
    {
        public static int CurrentShowingWindowIndex { get; set; }
        public static int CurrentShowedWindowIndex { get; set; }
    }
    
    public class BaseLauncherWindow<T> : BaseWindow<T> where T : BaseLauncherWindow<T>
    {
        public static int Index => Instance.Internal_Index;
        protected virtual int Internal_Index { get; }
        private float XShow => LauncherWindowsData.CurrentShowedWindowIndex > Internal_Index ? x : -x;
        private float XHide => LauncherWindowsData.CurrentShowingWindowIndex > Internal_Index ? x : -x;
        protected override ShowWindowOption ShowOption => ShowWindowOption.HideAllPrevious;
        protected override float DefaultAlpha => 1;
        private Vector2 startPivot;
        private float x;

        protected override void Init()
        {
            base.Init();
            var rect = RectTransform.rect;
            x = ((RectTransform)Canvas.rootCanvas.transform).rect.width / rect.width;
            var size = rect.size;
            RectTransform.anchorMin = LSVector2.half;
            RectTransform.anchorMax = LSVector2.half;
            RectTransform.sizeDelta = size;
            startPivot = RectTransform.pivot;
        }

        protected override void OnShowing()
        {
            base.OnShowing();
            LauncherWindowsData.CurrentShowingWindowIndex = Internal_Index;
        }
        
        protected override void OnShowed()
        {
            base.OnShowed();
            LauncherWindowsData.CurrentShowedWindowIndex = Internal_Index;
        }
        
        protected override Tween ShowAnim
        {
            get
            {
                var pivot = startPivot;
                pivot.x += XShow;
                RectTransform.pivot = pivot;
                var sequence = DOTween.Sequence()
                    .Insert(0, RectTransform.DOPivotX(startPivot.x, animDuration));
                return sequence;
            }
        }

        protected override Tween HideAnim
        {
            get
            {
                var sequence = DOTween.Sequence()
                    .Insert(0, RectTransform.DOPivotX(startPivot.x + XHide, animDuration));
            
                return sequence;
            }
        }
    }
}