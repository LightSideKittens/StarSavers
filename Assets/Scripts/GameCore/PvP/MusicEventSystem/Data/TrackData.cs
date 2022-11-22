using System;
using Newtonsoft.Json;

[Serializable]
public class TrackData
{
    public event Action Started;
    public event Action Playing;
    public event Action Ended;
    public event Action Completed;
    
    public NoteData[] notes;

    private int nextNote;
    private InAction<float> updater;

    public TrackData(string name, NoteData[] notes)
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

    [JsonIgnore] public NoteData CurrentNote => notes[nextNote];

    public void Update(in float currentTime)
    {
        updater(currentTime);
    }

    private void CheckStarted(in float currentTime)
    {
        if (currentTime > notes[nextNote].startTime)
        {
            Started?.Invoke();
            updater = CheckEnded;
        }
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
            
            nextNote++;

            if (nextNote == notes.Length)
            {
                Completed();
            }
            else
            {
                updater = CheckStarted;
                CheckStarted(currentTime);
            }
        }
    }

    public void SkipToTime(float time)
    {
        if (notes.Length == 0)
        {
            return;
        }
        
        while (time > notes[nextNote].startTime)
        {
            nextNote++;

            if (nextNote == notes.Length)
            {
                updater = UpdaterPlug;
                return;
            }
        }
    }

    private void UpdaterPlug(in float currentTime) => Completed();
}