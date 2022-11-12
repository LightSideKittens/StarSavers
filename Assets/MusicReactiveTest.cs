using System;
using DefaultNamespace;
using DefaultNamespace.MusicEventSystem;
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
        MainMusicData.GetTrack("Kick").Started += () =>
        {
            new CountDownTimer(0.1f, true).Stopped += () =>
            {
                DOTween.Kill(this);
                Camera.main.backgroundColor = Color.white;
                Camera.main.DOColor(Color.black, 0.4f).SetId(this);
                Camera.main.DOShakeRotation(duration, shakeStrength, shakeVibrato);
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
            time += Time.deltaTime;
            MainMusicData.Update(time);
        }
        
        if (isEnemyMusicStarted)
        {
            enemyTime += Time.deltaTime;
            EnemyMusicData.Update(enemyTime);
        }
    }
}
