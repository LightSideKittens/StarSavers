#if UNITY_EDITOR
namespace MusicEventSystem.Configs
{
    public partial class MusicData
    {
        public static void Editor_SetMusicName(string name)
        {
            musicName = name;
        }
    }
}
#endif
