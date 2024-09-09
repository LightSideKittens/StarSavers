using LSCore;

namespace StarSavers
{
    public partial class DebugData : BaseDebugData<DebugData>
    {
        public new static DebugData Config => BaseDebugData<DebugData>.Config;
        public bool needShowRadius;
        public bool serverEnabled;

        public static void Save() => Manager.Save();
    }
}