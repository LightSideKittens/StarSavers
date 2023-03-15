using System;
using MusicEventSystem.Configs;

namespace BeatRoyale
{
    public class ShortNoteListener : IDisposable
    {
        public event Action Started;
        public event Action Completed;
        private float offset;
        private ShortNoteListener(){}

        public static ShortNoteListener Listen(string key, float offset = 0)
        {
            var listener = new ShortNoteListener();
            listener.offset = MusicController.MusicOffset + offset;
            var shortTrack = MusicData.ShortTrackData;
            var track = shortTrack.GetTrack(key);
            track.Started += listener.OnStart;
            track.Completed += listener.OnComplete;
            
            return listener;
        }

        public ShortNoteListener OnStarted(Action action)
        {
            Started += action;
            return this;
        }
        
        public ShortNoteListener OnCompleted(Action action)
        {
            Completed += action;
            return this;
        }

        private void OnStart()
        {
            new CountDownTimer(offset, true).Stopped += OnStartStoped;
        }

        private void OnStartStoped()
        {
            Started?.Invoke();
        }
        
        private void OnComplete()
        {
            new CountDownTimer(offset, true).Stopped += OnCompleteStoped;
        }

        private void OnCompleteStoped()
        {
            Completed?.Invoke();
        }

        public void Dispose()
        {
            Started = null;
            Completed = null;
        }
    }
}