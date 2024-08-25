using System;
using Battle;
using Battle.Data;
using Battle.Windows;

namespace LSCore.BattleModule
{
    public partial class BattleWorld
    {
        [Serializable]
        public class BossWave : RaidConfig.WaveAction
        {
            public RaidConfig.BossData bossData;
            
            public override void OnStart()
            {
                BattleWindow.SplashText("BOSS");
                BattleWindow.BossHealth.maxValue = OpponentWorld.GetFullBossHealth(bossData);
                BattleWindow.BossHealth.value = OpponentWorld.GetBossHealth(bossData);
                BattleWindow.IsBossMode = true;
                OpponentWorld.UnleashKraken(bossData, OnWaveCompleted);
            }

            public override void OnComplete()
            {
                base.OnComplete();
                BattleWindow.IsBossMode = false;
            }
        }
    }
}