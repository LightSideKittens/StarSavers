using System;
using LSCore;
using LSCore.AnimationsModule.Animations;
using LSCore.Extensions;
using LSCore.LifecycleSystem;
using StarSavers;

namespace LGCore.UIModule.Quests
{
    [Serializable]
    public class DefeatEnemies : LifecycleObject.Handler
    {
        public const string defeatedEnemies = nameof(defeatedEnemies);
        
        public int count;
        public LSSlider slider;
        public SliderAnim anim;
        
        private int CurrentCount => PlayerStats.Config.defeatedEnemies;
        private int TargetCount => targetObjData[defeatedEnemies]!.ToObject<int>();
        private int LastCount => lastObjData[defeatedEnemies]?.ToObject<int>() ?? CurrentCount;

        protected override bool Check() => CurrentCount >= TargetCount;

        public override void BuildTargetData(RJToken questToken)
        {
            questToken[defeatedEnemies] = CurrentCount + count;
        }

        protected override void OnSetupView()
        {
            var lastCount = LastCount;
            var targetCount = TargetCount;

            slider.minValue = targetCount - count;
            slider.value = lastCount;
            slider.maxValue = targetCount;
            slider.OnlyDiff = true;
        }

        public override void OnShowed()
        {
            var currentCount = CurrentCount;
            
            if (lastObjData.CheckDiffAndSync<int>(defeatedEnemies, currentCount))
            {
                anim.endValue = currentCount;
                anim.Animate();
            }
        }
    }
}