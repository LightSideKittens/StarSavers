using System.Collections.Generic;
using Battle.Data;
using GameCore.Battle.Data;

namespace BeatRoyale
{
    public class MatchData
    {
        public class PlayerData
        {
            public CardDecks decks;
            public EntitiesProperties properties;
        }
        
        public static Dictionary<string, PlayerData> PlayerDataByUserId { get; } = new();

        public static void Clear()
        {
            PlayerDataByUserId.Clear();   
        }
    }
}