using System.Collections.Generic;
using Core.ConfigModule;
using UnityEngine;

namespace Battle.Data
{
    public class ChangedLevels : JsonBaseConfigData<ChangedLevels>
    {
        public Dictionary<string, int> Levels { get; set; } = new();
    }
    
    public class EditorLevels : JsonBaseConfigData<EditorLevels>
    {
        public HashSet<string> LevelsNames { get; set; } = new();
    }
}