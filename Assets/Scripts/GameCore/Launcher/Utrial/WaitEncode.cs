using UnityEngine;

namespace Launcher
{
    public class WaitEncode : CustomYieldInstruction
    {
        private JPGEncoder encoder;
        public override bool keepWaiting => !encoder.isDone;

        public WaitEncode(JPGEncoder encoder)
        {
            this.encoder = encoder;
        }
    }
}