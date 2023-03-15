using System;
using System.Collections.Generic;
using Battle.Data;
using Core.ConfigModule;
using GameCore.Battle.Data;

namespace BeatRoyale
{
    public class MatchPlayersData
    {
        private CardDecks decks;
        private EntitiesProperties properties;
        private CommonPlayerData playerData;
        private static Dictionary<string, MatchPlayersData> byUserId = new();
        public static int Count => byUserId.Count;

        private MatchPlayersData(CardDecks decks, EntitiesProperties properties, CommonPlayerData playerData)
        {
            this.decks = decks;
            this.properties = properties;
            this.playerData = playerData;
        }

        public static CardDecks GetDecks(string userId) => byUserId[userId].decks;
        public static EntitiesProperties GetProperties(string userId) => byUserId[userId].properties;
        public static CommonPlayerData GetPlayerData(string userId) => byUserId[userId].playerData;

        public static void Add(string userId, Action onSuccess)
        {
            RemotePlayerData<CardDecks>.Fetch(userId, decks =>
            {
                RemotePlayerData<EntitiesProperties>.Fetch(userId, properties =>
                {
                    RemotePlayerData<CommonPlayerData>.Fetch(userId, playerData =>
                    {
                        byUserId.Add(userId, new MatchPlayersData(decks, properties, playerData));
                        onSuccess();
                    });
                });
            });
        }

        public static void Clear()
        {
            byUserId.Clear();   
        }
    }
}