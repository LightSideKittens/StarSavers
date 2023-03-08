using System.Collections.Generic;
using Battle.Data;
using GameCore.Battle.Data;

namespace BeatRoyale
{
    public class MatchPlayersData
    {
        public CardDecks Decks { get; }
        public EntitiesProperties Properties { get; }
        public static Dictionary<string, MatchPlayersData> ByUserId { get; } = new();

        private MatchPlayersData(CardDecks decks, EntitiesProperties properties)
        {
            Decks = decks;
            Properties = properties;
        }

        public static void Add(string userId, CardDecks decks, EntitiesProperties properties)
        {
            ByUserId.Add(userId, new MatchPlayersData(decks, properties));
        }

        public static void Clear()
        {
            ByUserId.Clear();   
        }
    }
}