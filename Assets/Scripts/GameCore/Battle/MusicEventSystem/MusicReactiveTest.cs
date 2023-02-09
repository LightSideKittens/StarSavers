using System;
using Battle.Data;
using DG.Tweening;
using MusicEventSystem.Configs;
using UnityEngine;

public class MusicReactiveTest : MonoBehaviour
{
    private static Quaternion defaultCameraRot;
    public static event Action Started;
    private int count;
    private float time;
    private bool isMainMusicStarted;

    [SerializeField] private string musicName;
    [SerializeField] private float timeOffset;
    [SerializeField] private AudioSource source;
    
    [SerializeField] private BossesData bossesData;
    [SerializeField] private PassiveBulletsData passiveBullets;
    [SerializeField] private ActiveBulletsData activeBullets;
    [SerializeField] private EnemiesData enemies;
    
    [SerializeField] private Castle[] castles;
    
    [SerializeField] private float shakeStrength = 1;
    [SerializeField] private int shakeVibrato = 30;
    [SerializeField] private float duration = 0.3f;

    [SerializeField] private GameObject[] fires;
    [SerializeField] private Vector3 targetScale = new Vector3(5, 5, 5);
    [SerializeField] private float targetScaleSpeed = 3f;

    private void Awake()
    {
        MusicData.MusicName = musicName;
        time += timeOffset;
        source.time += timeOffset;
        MusicData.SkipToTime(timeOffset);
        defaultCameraRot = Camera.main.transform.rotation;
    }

    private void Start()
    {
        MusicData.ShortTrackData.GetTrack(SoundventTypes.ShortIV).Started += () =>
        {
            Time.timeScale = 1;

            new CountDownTimer(0.1f, true).Stopped += () =>
            {
                DOTween.Kill("Scale");
                DOTween.Kill("CameraShake");
                DOTween.Kill(this);
                ResetCameraPosition();
                Camera.main.DOShakeRotation(duration, shakeStrength, shakeVibrato).OnComplete(ResetCameraPosition).SetId("CameraShake");

                for (int i = 0; i < fires.Length; i++)
                {
                    fires[i].transform.DOScale(targetScale, 0.1f).SetId("Scale");
                }
            };
        };

        bossesData.Init();
        passiveBullets.Init();
        activeBullets.Init();
        enemies.Init();
    }

    public static void ResetCameraPosition()
    {
        Camera.main.transform.rotation = defaultCameraRot;
    }

    public void Update()
    {
        for (int i = 0; i < fires.Length; i++)
        {
            if (fires[i].transform.localScale.x > 1)
            {
                fires[i].transform.localScale -= Vector3.one * Time.deltaTime * targetScaleSpeed;
            }
        }
        
        if (Input.GetMouseButtonDown(0) && !isMainMusicStarted)
        {
            isMainMusicStarted = true;
            source.Play();
            Started?.Invoke();
        }
        
        if (isMainMusicStarted)
        {
            time += Time.deltaTime;
            MusicData.Update(time);
        }
    }
}
