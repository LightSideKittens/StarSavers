using GameCore.Attributes;
using UnityEngine;

namespace BeatRoyale.Windows
{
    internal partial class ControlPanel : BaseWindow<ControlPanel>
    {
        [ColoredField, SerializeField] private Tabs tabs;

        protected override void Init()
        {
            base.Init();
            tabs.Init();
        }
    }
}