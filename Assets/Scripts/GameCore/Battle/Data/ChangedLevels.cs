using System.Collections.Generic;
using Core.ConfigModule;

namespace Battle.Data
{
    public class ChangedLevels : JsonBaseConfigData<ChangedLevels>
    {
        protected override bool NeedAutoSave => false;
        public Dictionary<string, int> Levels { get; } = new();
    }
}