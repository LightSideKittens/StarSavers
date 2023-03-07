using System.Collections.Generic;
using Core.ConfigModule;

namespace GameCore.Battle.Data
{
    public class CardDecks : JsonBaseConfigData<CardDecks>
    {
        public List<string> Attack { get; } = new();
        public List<string> Defence { get; } = new();
        public List<string> Heroes { get; } = new();
    }
}