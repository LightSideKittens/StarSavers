using UnityEngine;

public class CountUpTimer : Timer.Base
{
    protected override bool IsIntUpdated => time > intTime;
    protected override bool IsTimeOver => time > Interval;
    
    protected override void ResetTime()
    {
        time = 0;
        intTime = 0;
    }

    protected override void UpdateTime()
    {
        time += deltaTime();
    }
    
    protected override void UpdateIntTime()
    {
        intTime++;
    }

    public CountUpTimer(float interval, bool needStartImmediately = false, bool isLoop = false, bool useUnscaleDeltaTime = false) : base(interval, needStartImmediately, isLoop, useUnscaleDeltaTime)
    {
    }
}