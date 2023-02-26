using System;
using System.Collections.Generic;
using Core.ConfigModule;

namespace Battle.Data
{
    public class EntitiesProperties : JsonBaseConfigData<EntitiesProperties>
    {
        public Dictionary<string, Dictionary<string, ValuePercent>> Properties { get; } = new();
    }
}