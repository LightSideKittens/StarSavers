using System;
using Newtonsoft.Json;

[Serializable]
public struct NoteData
{
    public float startTime;
    public float endTime;
    [JsonIgnore] public float Duration => endTime - startTime;

    public NoteData(float startTime, float endTime)
    {
        this.startTime = startTime;
        this.endTime = endTime;
    }
}