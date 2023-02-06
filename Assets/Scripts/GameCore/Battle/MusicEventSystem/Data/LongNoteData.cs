using System;
using Newtonsoft.Json;

[Serializable]
public struct LongNoteData
{
    public float startTime;
    public float endTime;
    [JsonIgnore] public float Duration => endTime - startTime;

    public LongNoteData(float startTime, float endTime)
    {
        this.startTime = startTime;
        this.endTime = endTime;
    }
}

[Serializable]
public struct ShortNoteData
{
    public float startTime;

    public ShortNoteData(float startTime)
    {
        this.startTime = startTime;
    }
}