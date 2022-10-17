using System;
using System.Collections.Generic;
using Core.ConfigModule;
using Newtonsoft.Json;

[Serializable]
public class MusicData : JsonBaseConfigData<MusicData>
{
    public static string musicName;
    
    [JsonProperty] private readonly Dictionary<string, TrackData> tracks = new Dictionary<string, TrackData>();
    private Dictionary<string, TrackData> tempTracks;
    private Action remover;

    public static void CreateTrack(string name, NoteData[] notes)
    {
        Config.tracks[name] = new TrackData(notes);
    }

    protected override void OnLoaded()
    {
        base.OnLoaded();
        InitTracks();
    }

    private void InitTracks()
    {
        tempTracks = new Dictionary<string, TrackData>(tracks);
        remover = CleanRemover;
        
        foreach (var track in tempTracks)
        {
            track.Value.Completed += () => remover += () => tempTracks.Remove(track.Key);
        }
    }

    protected override string FileName => musicName;

    public static void Update(in float currentTime)
    {
        var config = Config;

        foreach (var track in config.tempTracks.Values)
        {
            track.Update(currentTime);
        }

        config.remover();
    }

    public static TrackData GetTrack(string key)
    {
        return Config.tracks[key];
    }

    private void CleanRemover()
    {
        remover = CleanRemover;
    }
}