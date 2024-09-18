using LSCore;
using LSCore.AnimationsModule.Animations;
using LSCore.QuestModule;
using StarSavers;

namespace LGCore.UIModule.Quests
{
    public class DefeatEnemies : Quest.Handler
    {
        public const string defeatedEnemies = nameof(defeatedEnemies);
        
        public int count;
        public LSSlider slider;
        public SliderAnim anim;
        
        private int CurrentCount => PlayerStats.Config.defeatedEnemies;
        private int TargetCount => targetQuestData[defeatedEnemies]!.ToObject<int>();
        private int LastCount => lastQuestData[defeatedEnemies]?.ToObject<int>() ?? 0;

        protected override bool Check() => CurrentCount >= TargetCount;

        public override void BuildTargetData(RJToken questToken)
        {
            questToken[defeatedEnemies] = CurrentCount + count;
        }

        public override void OnSetupView()
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
            
            if (CheckDiffAndSync<int>(defeatedEnemies, currentCount))
            {
                anim.endValue = currentCount;
                anim.Animate();
            }
        }
    }
}