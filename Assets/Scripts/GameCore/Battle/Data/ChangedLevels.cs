using System.Collections.Generic;
using Core.ConfigModule;
using static Core.ConfigModule.FolderNames;
using static UnityEngine.Application;

namespace Battle.Data
{
    public class ChangedLevels : JsonBaseConfigData<ChangedLevels>
    {
        protected override string DefaultFolderName => isPlaying ? SaveData : Remote;
        public Dictionary<string, int> Levels { get; } = new();
    }
}