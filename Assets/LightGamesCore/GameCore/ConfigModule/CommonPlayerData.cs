using Core.ConfigModule;
using Newtonsoft.Json;

namespace BeatRoyale
{
    public class CommonPlayerData : JsonBaseConfigData<CommonPlayerData>
    {
        [JsonProperty] private string userId;

        public static string UserId
        {
            get => Config.userId;
            set => Config.userId = value;
        }
    }
}