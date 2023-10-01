using LSCore;

namespace BeatRoyale
{
    public class DebugData : BaseDebugData<DebugData>
    {
        public static DebugData Config => BaseDebugData<DebugData>.Config;
        public bool needShowRadius;
        public bool serverEnabled;
        public bool infinityMana;

        public static void Save() => BaseDebugData<DebugData>.Save();
    }
}