using UnityEngine;

public class CountDownTimer : Timer.Base
{
    protected override bool IsIntUpdated => time < intTime;
    protected override bool IsTimeOver => time < 0;

    protected override void ResetTime()
    {
        time = interval;
        intTime = Mathf.CeilToInt(interval);
    }

    protected override void UpdateTime()
    {
        time -= deltaTime();
    }
    
    protected override void UpdateIntTime()
    {
        intTime--;
    }

    public CountDownTimer(float interval, bool needStartImmediately = true, bool isLoop = false, bool useUnscaleDeltaTime = false) : base(interval, needStartImmediately, isLoop, useUnscaleDeltaTime)
    {
    }
}
