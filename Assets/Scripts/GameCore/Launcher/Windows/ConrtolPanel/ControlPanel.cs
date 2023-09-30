using GameCore.Attributes;
using LSCore;
using UnityEngine;

namespace BeatRoyale.Windows
{
    internal partial class ControlPanel : BaseWindow<ControlPanel>
    {
        [ColoredField, SerializeField] private Tabs tabs;
        public static int CurrentShowingWindowIndex { get; set; }
        public static int CurrentShowedWindowIndex { get; set; }
        public static Rect Rect { get; private set; }
        public static float LeftX { get; private set; }
        public static float RightX { get; private set; }


        protected override void Init()
        {
            base.Init();
            Rect = ((RectTransform) transform).rect;
            var halfWidth = Rect.width;
            LeftX = halfWidth * -1;
            RightX = halfWidth * 1;
            
            tabs.Init();
        }
    }
}