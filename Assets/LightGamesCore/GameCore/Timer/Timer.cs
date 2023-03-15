using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class Timer : MonoBehaviour
{
    public abstract class Base
    {
        public event Action Stopped;
        public event Action<float> NormalizeUpdated;
        public event Action<float> Updated;
        public event Action<int> IntUpdated;
        public event Action Elapsed;
        public event Action Killed;
        
        private static Action pauseAll;
        private static Action resumeAll;
        private static Action<Base, float, bool, bool, bool> start;
        protected Func<float> deltaTime;
        public bool isLoop;
        
        protected float time;
        protected int intTime = 1;
        protected float interval;
        protected bool isStarted;
        protected bool isPaused;

        public float Interval
        {
            get
            {
                return interval;
            }
            set
            {
                interval = value;
            }
        }

        protected abstract bool IsIntUpdated { get; }
        protected abstract bool IsTimeOver { get; }

        static Base()
        {
            start = FirstConstructor;
        }

        protected Base(float interval, bool needStartImmediately = false, bool isLoop = false, bool useUnscaleDeltaTime = false)
        {
            start(this, interval, needStartImmediately, isLoop, useUnscaleDeltaTime);
        }

        private static void DefaultConstructor(Base timer, float interval, bool needStartImmediately = false, bool isLoop = false, bool useUnscaleDeltaTime = false)
        {
            ThrowException();
            timer.Interval = interval;
            timer.isLoop = isLoop;
            timer.deltaTime = useUnscaleDeltaTime == false ? () => Time.deltaTime : () => Time.unscaledDeltaTime;

            if (needStartImmediately)
            {
                timer.Restart();
            }
        }
        
        private static void FirstConstructor(Base timer, float interval, bool needStartImmediately = false, bool isLoop = false, bool useUnscaleDeltaTime = false)
        {
            new GameObject("Timer").AddComponent<Timer>();
            DefaultConstructor(timer, interval, needStartImmediately, isLoop, useUnscaleDeltaTime);
            start = DefaultConstructor;
        }

        [Conditional("UNITY_EDITOR")]
        private static void ThrowException()
        {
            if (IsCreated == false)
            {
                throw new Exception(
                    $"[{nameof(Timer)}]" +
                    $" Timer is not created." +
                    $" Check that the {nameof(Timer)} assembly was added to " +
                    $"Assets/Resources/AssemdefsForStaticReseter.asset");
            }
        }
        
        public void Start()
        {
            if (isStarted == false)
            {
                ResetTime();
                UnityUpdated += Update;
                pauseAll += Pause;
                resumeAll += Resume;
                isStarted = true;
            }
        }
        
        public void Restart()
        {
            if (isStarted == false)
            {
                Start();
            }
            else
            {
                ResetTime();
            }
        }

        public void Pause()
        {
            if (isStarted && isPaused == false)
            {
                UnityUpdated -= Update;
                isPaused = true;
            }
        }

        public static void PauseAll()
        {
            pauseAll?.Invoke();
        }

        public static void ResumeAll()
        {
            resumeAll?.Invoke();
        }

        public void Resume()
        {
            if (isStarted && isPaused)
            {
                UnityUpdated += Update;
                isPaused = false;
            }
        }
    
        public void Stop()
        {
            if (isStarted)
            {
                isStarted = false;
                ResetTime();
                UnityUpdated -= Update;
                Stopped?.Invoke();
                pauseAll -= Pause;
                resumeAll -= Resume;
            }
        }

        public void Kill()
        {
            UnityUpdated -= Update;
            Updated = null;
            NormalizeUpdated = null;
            IntUpdated = null;
            Stopped = null;
            pauseAll -= Pause;
            resumeAll -= Resume;
            Killed?.Invoke();
        }
        
        private void Update()
        {
            Updated?.Invoke(time);
            NormalizeUpdated?.Invoke(Mathf.InverseLerp(0, interval, time));

            UpdateTime();

            if (IsTimeOver)
            {
                OnTimeOver();
                return;
            }
            
            if (IsIntUpdated)
            {
                IntUpdated?.Invoke(intTime);
                UpdateIntTime();
            }
        }
        
        private void OnTimeOver()
        {
            if (isLoop)
            {
                ResetTime();
                Elapsed?.Invoke();
            }
            else
            {
                Stop();
            }
        }

        protected abstract void ResetTime();
        protected abstract void UpdateTime();
        protected abstract void UpdateIntTime();
    }

    public static bool IsCreated => instance != null;
    private static event Action UnityUpdated;
    private static Timer instance;

    private void Awake()
    {
        DontDestroyOnLoad(this);
        instance = this;
    }

    public static void StartCoroutinee(IEnumerator enumerator)
    { 
        instance.StartCoroutine(enumerator);
    }

    private void Update()
    {
        UnityUpdated?.Invoke();
    }
}

public static class TimerExtensions
{
    private static readonly Dictionary<int, Timer.Base> timers = new Dictionary<int, Timer.Base>();
    public static Timer.Base SetId(this Timer.Base timer, object id)
    {
        timers[id.GetHashCode()] = timer;

        return timer;
    }

    public static void Kill(object id)
    {
        var key = id.GetHashCode();

        if (timers.TryGetValue(key, out var timer))
        {
            timer.Kill();
            timers.Remove(key);
        }
    }
}
