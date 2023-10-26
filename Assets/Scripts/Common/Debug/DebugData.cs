using LSCore;

namespace BeatHeroes
{
    public partial class DebugData : BaseDebugData<DebugData>
    {
        public static DebugData Config => BaseDebugData<DebugData>.Config;
        public bool needShowRadius;
        public bool serverEnabled;

        public static void Save() => BaseDebugData<DebugData>.Save();
    }
}