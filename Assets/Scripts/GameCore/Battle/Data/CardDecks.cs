using System.Collections.Generic;
using Core.ConfigModule;

namespace GameCore.Battle.Data
{
    public class CardDecks : JsonBaseConfigData<CardDecks>
    {
        public List<string> Attack { get; } = new()
        {
            "Potion", "Rage", "Bomby", "Stanley", "Frozer", "Manny", "Archer", "Knight",
        };
        
        public List<string> Defence { get; } = new()
        {
            "Cloudy", "Goblin", "Drago", "Stoneval"
        };
        
        public List<string> Heroes { get; } = new()
        {
            "Raccoon", "Madina", "Dumbledore", "Valkyrie" 
        };
    }
}