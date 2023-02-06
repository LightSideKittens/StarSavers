using System;
using Newtonsoft.Json;

[Serializable]
public abstract class BaseTrackData
{
    public event Action Started;
    public event Action Completed;
    
    protected void Complete()
    {
        Completed?.Invoke();
    }
    
    protected void Start()
    {
        Started?.Invoke();
    }

    public abstract void Update(in float currentTime);
    public abstract void SkipToTime(in float time);
}

[Serializable]
public abstract class BaseTrackData<T> : BaseTrackData where T : struct
{
    public T[] notes;
    protected int nextNote;
    protected InAction<float> updater;

    public BaseTrackData(string name, T[] notes)
    {
        this.notes = notes;
        
        if (notes.Length == 0)
        {
            updater = UpdaterPlug;
        }
        else
        {
            updater = CheckStarted;
        }
    }

    protected abstract void CheckStarted(in float currentTime);

    protected void UpdaterPlug(in float currentTime) => Complete();
    
    protected void CheckComplete(in float currentTime)
    {
        nextNote++;

        if (nextNote < notes.Length)
        {
            updater = CheckStarted;
            CheckStarted(currentTime);
        }
        else
        {
            nextNote--;
            Complete();
        }
    }
    
    public override void SkipToTime(in float time)
    {
        if (notes.Length == 0)
        {
            return;
        }
        
        while (time > GetStartTime(nextNote))
        {
            nextNote++;

            if (nextNote >= notes.Length)
            {
                updater = UpdaterPlug;
                return;
            }
        }
    }

    protected abstract float GetStartTime(in int index);

    public override void Update(in float currentTime)
    {
        updater(currentTime);
    }
}

[Serializable]
public class ShortNoteTrackData : BaseTrackData<ShortNoteData>
{
    public ShortNoteTrackData(string name, ShortNoteData[] notes) : base(name, notes) { }

    protected override void CheckStarted(in float currentTime)
    {
        if (currentTime > GetStartTime(nextNote))
        {
            Start();
            CheckComplete(currentTime);
        }
    }

    protected override float GetStartTime(in int index)
    {
        return notes[index].startTime;
    }
}

[Serializable]
public class LongNoteTrackData : BaseTrackData<LongNoteData>
{
    public event Action Playing;
    public event Action Ended;

    public LongNoteTrackData(string name, LongNoteData[] notes) : base(name, notes) { }

    [JsonIgnore] public LongNoteData CurrentLongNote => notes[nextNote];


    protected override void CheckStarted(in float currentTime)
    {
        if (currentTime > GetStartTime(nextNote))
        {
            Start();
            updater = CheckEnded;
        }
    }

    protected override float GetStartTime(in int index)
    {
        return notes[index].startTime;
    }

    private void CheckEnded(in float currentTime)
    {
        if (currentTime < notes[nextNote].endTime)
        {
            Playing?.Invoke();
        }
        else
        {
            Ended?.Invoke();
            CheckComplete(currentTime);
        }
    }
}