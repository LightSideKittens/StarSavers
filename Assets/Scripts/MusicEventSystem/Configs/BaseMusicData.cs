using System;
using System.Collections.Generic;
using Core.ConfigModule;
using Newtonsoft.Json;

[Serializable]
public abstract class BaseMusicData<T> : JsonBaseConfigData<T> where T : BaseMusicData<T>, new()
{
    private static string musicName;
    
    protected override string FileName => musicName;
    protected override string FolderName => "MusicData";
    
    public static string MusicName
    {
        get
        {
            return musicName;
        }
        set
        {
            LoadOnNextAccess();
            musicName = value;
        }
    }
    
    [JsonProperty] private readonly Dictionary<string, TrackData> tracks = new Dictionary<string, TrackData>();
    [JsonProperty] public int BPM { get; private set; }
    private Dictionary<string, TrackData> tempTracks;
    private Action remover;

    public static void Clear()
    {
        Config.tracks.Clear();;
    }

    public static void SetTrack(string name, NoteData[] notes)
    {
        Config.tracks[name] = new TrackData(name, notes);
    }

    public static void SkipToTime(float time)
    {
        var config = Config;
        var tracks = config.tempTracks.Values;
        
        foreach (var track in tracks)
        {
            track.SkipToTime(time);
        }
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

    public static void Update(in float currentTime)
    {
        var config = Config;
        var tracks = config.tempTracks.Values;

        foreach (var track in tracks)
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