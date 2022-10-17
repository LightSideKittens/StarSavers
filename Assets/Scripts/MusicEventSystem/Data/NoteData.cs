using System;

[Serializable]
public struct NoteData
{
    public float startTime;
    public float endTime;

    public NoteData(float startTime, float endTime)
    {
        this.startTime = startTime;
        this.endTime = endTime;
    }
}