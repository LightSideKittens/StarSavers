using GameCore.Attributes;
using UnityEngine;

namespace BeatRoyale.Windows
{
    internal partial class ControlPanel : BaseWindow<ControlPanel>
    {
        [ColoredField, SerializeField] private Tabs tabs;
        public static int CurrentShowingWindowIndex { get; set; }
        public static int CurrentShowedWindowIndex { get; set; }
        public static float LeftX { get; private set; }
        public static float RightX { get; private set; }


        protected override void Init()
        {
            base.Init();
            tabs.Init();
            var halfWidth = ((RectTransform) transform).rect.width;
            LeftX = halfWidth * -1;
            RightX = halfWidth * 1;
        }
    }
}