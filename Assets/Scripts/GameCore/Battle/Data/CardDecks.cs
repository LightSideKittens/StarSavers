using System.Collections.Generic;
using LGCore.ConfigModule;

namespace GameCore.Battle.Data
{
    public class CardDecks : BaseConfig<CardDecks>
    {
        public List<string> Attack { get; } = new();
        public List<string> Defence { get; } = new();
        public List<string> Heroes { get; } = new();
    }
}