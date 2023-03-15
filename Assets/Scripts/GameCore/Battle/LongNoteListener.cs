using System;
using MusicEventSystem.Configs;

namespace BeatRoyale
{
    public class LongNoteListener
    {
        public event Action Started;
        public event Action Ended;
        public event Action Completed;
        private float offset;

        public static LongNoteListener Listen(string key, float offset = 0)
        {
            var listener = new LongNoteListener();
            listener.offset = MusicController.MusicOffset + offset;
            var shortTrack = MusicData.LongTrackData;
            var track = shortTrack.GetTrack(key);
            track.Started += listener.OnStart;
            track.Ended += listener.OnEnd;
            track.Completed += listener.OnComplete;
            
            return listener;
        }

        private void OnStart()
        {
            new CountDownTimer(offset, true).Stopped += OnStartStoped;
        }

        private void OnStartStoped()
        {
            Started?.Invoke();
        }
        
        private void OnEnd()
        {
            new CountDownTimer(offset, true).Stopped += OnEndStoped;
        }

        private void OnEndStoped()
        {
            Ended?.Invoke();
        }
        
        private void OnComplete()
        {
            new CountDownTimer(offset, true).Stopped += OnCompleteStoped;
        }

        private void OnCompleteStoped()
        {
            Completed?.Invoke();
        }
    }
}