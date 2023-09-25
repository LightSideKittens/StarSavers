using System;
using CAS;
using Sirenix.OdinInspector;
using static LGCore.SDKManagement.Advertisement.Ads.Reward;

namespace LGCore.SDKManagement.Advertisement.CAS
{
    [Serializable]
    [InlineProperty(LabelWidth = 30)]
    internal class Reward : BaseAdapter
    {
        protected override bool HasId => false;
        private Action<Result> callback;

        protected override void Internal_Init()
        {
            var manager = CASInitializer.Manager;
            Events.Reward.Manager = manager;
            manager.OnRewardedAdCompleted += OnRewardAdCompleted;
            manager.OnRewardedAdFailedToShow += OnRewardAdFailed;
        }

        private void OnRewardAdFailed(string error) => callback?.Invoke(Result.FailedDisplay);
        private void OnRewardAdCompleted() => callback?.Invoke(Result.Received);
        
        public override void Show(Action<Result> onResult)
        {
            callback = onResult;
            callback += ResetCallback;
            CASInitializer.Manager.ShowAd(AdType.Rewarded);
        }
        
        private void ResetCallback(Result _) => callback = null;
    }
}