using LSCore;
using LSCore.ConfigModule;

namespace StarSavers
{
    public partial class DebugData : BaseDebugData<DebugData>
    {
        public new static DebugData Config => BaseDebugData<DebugData>.Config;
        public bool needShowRadius;
        public bool serverEnabled;
        public new static void Save() => ConfigUtils.Save<DebugData>();
    }
}