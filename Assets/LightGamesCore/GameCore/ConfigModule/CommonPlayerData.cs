using Core.ConfigModule;
using Newtonsoft.Json;
using UnityEngine;

namespace BeatRoyale
{
    public class CommonPlayerData : JsonBaseConfigData<CommonPlayerData>
    {
        [JsonProperty] private string userId;
        [JsonProperty] private string nickName;
        private static string[] nickNames = new[]
        {
            "Captain Crunchwrap",
            "Sir Spam-a-lot",
            "Princess Pudding pop",
            "Count Quackula",
            "Duke of Deliciousness",
            "Lady Lollygag",
            "Baron Von Burrito",
            "The Great Gatsbyburger",
            "Dr. Doughnutstein",
            "The Burgermeister",
        };

        public static string UserId
        {
            get => Config.userId;
            set
            {
                var config = Config;
                config.userId = value;
                config.nickName = nickNames[Random.Range(0, nickNames.Length)];
            }
        }
        
        public static string NickName
        {
            get => Config.nickName;
            set => Config.nickName = value;
        }
    }
}