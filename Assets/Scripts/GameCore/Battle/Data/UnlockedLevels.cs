using System;
using System.Collections.Generic;
using Core.ConfigModule;

namespace Battle.Data
{
    public class UnlockedLevels : JsonBaseConfigData<UnlockedLevels>
    {
        public Dictionary<string, int> Levels { get; set; } = new Dictionary<string, int>()
        {
            {GameScopes.Raccoon, 1},
            {GameScopes.Dumbledore, 1},
        };
    }
}