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

        protected override void Init()
        {
            base.Init();
            tabs.Init();
        }
    }
}