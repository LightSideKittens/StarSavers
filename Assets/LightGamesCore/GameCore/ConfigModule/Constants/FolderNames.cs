using UnityEngine;

namespace Core.ConfigModule
{
    public static class FolderNames
    {
        public const string Configs = nameof(Configs);
        public const string DefaultSaveData = nameof(DefaultSaveData) + "/" + nameof(Resources);
        public const string SaveData = nameof(SaveData);
        public const string Remote = nameof(Remote);
        public const string EditorConfigs = nameof(EditorConfigs);
    }
}
