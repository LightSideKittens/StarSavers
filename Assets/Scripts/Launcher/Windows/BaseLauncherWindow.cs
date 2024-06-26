using DG.Tweening;
using LSCore;
using LSCore.AnimationsModule;
using LSCore.AnimationsModule.Animations;
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
        private Vector2 startPivot;
        private float x;
        private AnimSequencer animation;
        private PivotPosXAnim anim;

        protected override void Init()
        {
            base.Init();
            animation = ((InOutShowHideAnim)showHideAnim).animation;
            anim = animation.GetAnim<PivotPosXAnim>();
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
                anim.startValue = pivot.x;
                anim.endValue = startPivot.x;
                return animation.Animate();
            }
        }

        protected override Tween HideAnim
        {
            get
            {
                anim.endValue = startPivot.x + XHide;
                return animation.Animate();
            }
        }
    }
}