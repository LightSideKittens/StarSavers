using static LGCore.SDKManagement.Advertisement.Ads.Reward;

namespace LGCore.SDKManagement.Advertisement
{
    public static class RewardExtensions
    {
        public static bool IsReceived(this Result result) => result == Result.Received;

        public static bool IsClosed(this Result result) => result == Result.Closed;

        public static bool IsFailedDisplay(this Result result) => result == Result.FailedDisplay;
    }
}