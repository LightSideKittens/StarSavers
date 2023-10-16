/*using System;
using System.Collections.Generic;
using System.Linq;
using Battle.Data;
using Core.ConfigModule;
using Core.Server;
using GameCore.Battle.Data;

namespace BeatHeroes
{
    public class MatchData
    {
        private CardDecks decks;
        private EntiProps properties;
        private User user;
        private static Dictionary<string, MatchData> byUserId = new();
        public static int Count => byUserId.Count;

        private MatchData(CardDecks decks, EntiProps properties, User user)
        {
            this.decks = decks;
            this.properties = properties;
            this.user = user;
        }

        public static string OpponentUserId => User.ServerEnabled ? byUserId.ElementAt(0).Key : "Self";

        public static CardDecks GetDecks(string userId) => userId == User.Id ? CardDecks.Config : byUserId[userId].decks;
        public static EntiProps GetProperties(string userId) => userId == User.Id ? EntiProps.Config : byUserId[userId].properties;
        public static User GetUser(string userId) => userId == User.Id ? User.Config : byUserId[userId].user;

        public static void Add(string userId, Action onSuccess)
        {
            UserDatabase<CardDecks>.Fetch(userId, decks =>
            {
                UserDatabase<EntiProps>.Fetch(userId, properties =>
                {
                    UserDatabase<User>.Fetch(userId, playerData =>
                    {
                        byUserId.Add(User.ServerEnabled ? userId : "Self", new MatchData(decks, properties, playerData));
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
}*/