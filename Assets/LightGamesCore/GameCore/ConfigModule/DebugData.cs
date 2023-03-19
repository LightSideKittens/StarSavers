#if DEBUG

namespace Core.ConfigModule
{
    public class DebugData : JsonBaseConfigData<DebugData>
    {
        protected override string FolderName => "DebugConfigs";
        public bool needShowRadius;
        public bool serverEnabled = true;
        public bool infinityMana;
    }
}
#endif