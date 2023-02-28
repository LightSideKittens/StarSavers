using System;
using Core.ConfigModule;
using Newtonsoft.Json;

namespace MusicEventSystem.Configs
{
    public partial class MusicData : JsonBaseConfigData<MusicData>
    {
        public const float BPMStep = 0.5f;
        private static string musicName;
        private static float lastCurrentTime;
        public static event Action BPMReached;
        public static event Action Loaded;
        [JsonProperty(Order = 0)] private NoteMusicData<LongNoteTrackData> longNoteTrack = new();
        [JsonProperty(Order = 1)] private NoteMusicData<ShortNoteTrackData> shortNoteTrack = new();
        public static NoteMusicData<LongNoteTrackData>.TracksData LongTrackData => Config.longNoteTrack.tracksData;
        public static NoteMusicData<ShortNoteTrackData>.TracksData ShortTrackData => Config.shortNoteTrack.tracksData;

        protected override string FileName => musicName;

        public static string MusicName
        {
            get => musicName;
            set
            {
                LoadOnNextAccess();
                musicName = value;
            }
        }
        
#if UNITY_EDITOR
        public static void Editor_SetMusicName(string name)
        {
            musicName = name;
        }
#endif

        protected override void OnLoaded()
        {
            base.OnLoaded();
            Loaded?.Invoke();
        }

        public static void Clear()
        {
            var config = Config;
            config.shortNoteTrack.Clear();
            config.longNoteTrack.Clear();
        }
    
        public static void SkipToTime(in float time)
        {
            var config = Config;
            config.shortNoteTrack.Update(in time);
            config.longNoteTrack.Update(in time);
        }
    
        public static void Update(in float currentTime)
        {
            var config = Config;
            config.shortNoteTrack.Update(in currentTime);
            config.longNoteTrack.Update(in currentTime);
        }
    }
}