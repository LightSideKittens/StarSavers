using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace MusicEventSystem.Configs
{
    public partial class MusicData
    {
        [Serializable]
        public class NoteMusicData<T1> where T1 : BaseTrackData
        {
            [Serializable]
            public struct TracksData
            {
                private readonly Dictionary<string, T1> tracks;

                public TracksData(Dictionary<string, T1> tracks)
                {
                    this.tracks = tracks;
                }
                
                public T1 GetTrack(string key)
                {
                    return tracks[key];
                }

                public void SetTrack(string name, T1 trackData)
                {
                    tracks[name] = trackData;
                }
            }
            
            [JsonProperty] private readonly Dictionary<string, T1> tracks = new();
            private Dictionary<string, T1> tempTracks;
            private Action remover;
            [JsonIgnore] public TracksData tracksData;

            public NoteMusicData()
            {
                Loaded += InitTracks;
                tracksData = new TracksData(tracks);
            }

            public void Clear()
            {
                tracks.Clear();
            }

            public void SkipToTime(float time)
            {
                var tracks = tempTracks.Values;

                foreach (var track in tracks)
                {
                    track.SkipToTime(time);
                }
            }

            private void InitTracks()
            {
                Loaded -= InitTracks;
                tempTracks = new Dictionary<string, T1>(tracks);
                remover = CleanRemover;

                foreach (var track in tempTracks)
                {
                    track.Value.Completed += () => remover += () => tempTracks.Remove(track.Key);
                }
            }

            public void Update(in float currentTime)
            {
                var tracks = tempTracks.Values;

                foreach (var track in tracks)
                {
                    track.Update(currentTime);
                }

                remover();

                if (currentTime >= lastCurrentTime + BPMStep)
                {
                    BPMReached?.Invoke();
                    lastCurrentTime = currentTime;
                }
            }

            private void CleanRemover()
            {
                remover = CleanRemover;
            }
        }
    }
}