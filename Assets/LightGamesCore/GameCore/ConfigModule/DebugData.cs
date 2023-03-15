#if DEBUG
using Core.ConfigModule;

namespace BeatRoyale
{
    public class DebugData : JsonBaseConfigData<DebugData>
    {
        protected override string FolderName => "DebugConfigs";
        public bool needShowRadius;
        public bool serverEnabled = true;
    }
}
#endif