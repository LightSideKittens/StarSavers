using DG.Tweening;
using MusicEventSystem.Configs;
using UnityEngine;

public class MusicReactiveTest : MonoBehaviour
{
    private Color[] colors = new []{ Color.white, Color.gray, Color.black, Color.yellow,};
    private int count;
    private float time;
    private float enemyTime;
    private bool isMainMusicStarted;
    private bool isEnemyMusicStarted;

    [SerializeField] private string musicName;
    [SerializeField] private float timeOffset;
    [SerializeField] private AudioSource source;
    [SerializeField] private EnemyBulletPairs pairs;
    [SerializeField] private Castle[] castles;
    
    [SerializeField] private float shakeStrength = 1;
    [SerializeField] private int shakeVibrato = 30;
    [SerializeField] private float duration = 0.3f;

    private void Awake()
    {
        EnemyMusicData.MusicName = musicName;
        MainMusicData.MusicName = musicName;
        time += timeOffset;
        enemyTime += timeOffset;
        source.time += timeOffset;
        MainMusicData.SkipToTime(timeOffset);
        EnemyMusicData.SkipToTime(timeOffset);
    }

    private void Start()
    {
        MainMusicData.GetTrack(SoundTypes.Kick).Started += () =>
        {
            Time.timeScale = 1;

            new CountDownTimer(0.1f, true).Stopped += () =>
            {
                DOTween.Kill("CameraShake");
                DOTween.Kill(this);
                Camera.main.backgroundColor = Color.white;
                Camera.main.DOColor(Color.black, 0.4f).SetId(this);
                Camera.main.DOShakeRotation(duration, shakeStrength, shakeVibrato).SetId("CameraShake");
            };
        };

        MainMusicData.GetTrack(SoundTypes.ClapSnare).Started += () =>
        {
            Time.timeScale = 1;
            new CountUpTimer(0.2f, true, useUnscaleDeltaTime: true).NormalizeUpdated += value =>
            {
                Time.timeScale = 1 - 0.8f * value;
            };
        };
        
        pairs.Init();

        for (int i = 0; i < castles.Length; i++)
        {
            castles[i].Init();
        }
    }

    public void Update()
    {
        if (Input.GetMouseButtonDown(0) && isEnemyMusicStarted == false)
        {
            isEnemyMusicStarted = true;
            new CountDownTimer(2, true).Stopped += () =>
            {
                isMainMusicStarted = true;
                source.Play();
            };
        }
        
        if (isMainMusicStarted)
        {
            time += Time.unscaledDeltaTime;
            MainMusicData.Update(time);
        }
        
        if (isEnemyMusicStarted)
        {
            enemyTime += Time.unscaledDeltaTime;
            EnemyMusicData.Update(enemyTime);
        }
    }
}
