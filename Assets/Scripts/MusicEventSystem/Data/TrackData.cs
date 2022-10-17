using System;

[Serializable]
public class TrackData
{
    public NoteData[] notes;
    private int nextNote;

    public event Action Started;
    public event Action Playing;
    public event Action Ended;
    public event Action Completed;
    private InAction<float> updater;

    public TrackData(NoteData[] notes)
    {
        this.notes = notes;
        updater = CheckStarted;
    }

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
            }
        }
    }
}