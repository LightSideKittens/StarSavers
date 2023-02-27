using System.Collections.Generic;
using Core.ConfigModule;

namespace Battle.Data
{
    public class EditorLevels : JsonBaseConfigData<EditorLevels>
    {
        protected override bool NeedAutoSave => false;
        public HashSet<string> LevelsNames { get; } = new();
    }
}