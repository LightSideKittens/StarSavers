using Battle;

public class BassCastle : Castle
{
    protected override void OnDamageEnemy<T>(Enemy enemy)
    {
        var soundType = currentSoundType;

        var time = MusicReactiveTest.MusicOffset - bulletFlyDuration + 0.1f;
        
        new CountDownTimer(time, true).Stopped += () =>
        {
            BassBullet.Create<T>(this, soundType, bulletFlyDuration, enemy, GetFX(soundType));
        };
    }
}
