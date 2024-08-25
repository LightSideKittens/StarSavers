using System;
using Battle.Data;
using LSCore.Async;

namespace LSCore.BattleModule
{
    public partial class BattleWorld
    {
        [Serializable]
        public class Break : RaidConfig.WaveAction
        {
            public float duration;
            
            public override void OnStart()
            {
                Wait.Delay(duration, OnWaveCompleted);
            }
        }
    }
}