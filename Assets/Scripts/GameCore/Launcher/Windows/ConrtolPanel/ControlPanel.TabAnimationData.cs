using System;

namespace BeatRoyale.Windows
{
    internal partial class ControlPanel
    {
        [Serializable]
        private struct TabAnimationData
        {
            public float duration;
            public float widthOffset;
            public float heightOffset;
        }
    }
}