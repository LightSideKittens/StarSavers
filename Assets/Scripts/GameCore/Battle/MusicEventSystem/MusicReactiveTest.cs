using System;
using Battle.ConfigsSO;
using DG.Tweening;
using MusicEventSystem.Configs;
using PathCreation;
using UnityEngine;

public class MusicReactiveTest : MonoBehaviour
{
    private static Quaternion defaultCameraRot;
    public static event Action Started;
    public static PathCreator Path { get; private set; }
    public static Vector3[] PathPoints { get; private set; }

    private Color[] colors = new []{ Color.white, Color.gray, Color.black, Color.yellow,};
    private int count;
    private float time;
    private bool isMainMusicStarted;
    public static float MusicOffset { get; private set; }

    [SerializeField] private string musicName;
    [SerializeField] private float timeOffset;
    [SerializeField] private PathCreator path;
    [SerializeField] private AudioSource source;
    [SerializeField] private BossesData bossesData;
    [SerializeField] private PassiveBulletsData passiveBullets;
    [SerializeField] private ActiveBulletsData activeBullets;
    [SerializeField] private EnemiesData enemies;
    [SerializeField] private Castle[] castles;
    
    [SerializeField] private float shakeStrength = 1;
    [SerializeField] private int shakeVibrato = 30;
    [SerializeField] private float duration = 0.3f;
    [SerializeField] private float musicOffset = 1f;

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
        MusicOffset = musicOffset;
        Path = path;
        PathPoints = new Vector3[path.path.NumPoints];

        for (int i = 0; i < path.path.NumPoints; i++)
        {
            PathPoints[i] = path.path.GetPoint(i);
        }
    }

    private void Start()
    {
        MusicData.ShortTrackData.GetTrack(SoundventTypes.ShortIV).Started += () =>
        {
            Time.timeScale = 1;

            new CountDownTimer(musicOffset + 0.1f, true).Stopped += () =>
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

        /*MainMusicData.GetTrack(SoundTypes.ClapSnare).Started += () =>
        {
            Time.timeScale = 1;
            new CountUpTimer(0.2f, true, useUnscaleDeltaTime: true).NormalizeUpdated += value =>
            {
                Time.timeScale = 1 - 0.8f * value;
            };
        };*/
        
        bossesData.Init();
        passiveBullets.Init();
        activeBullets.Init();
        enemies.Init();

        for (int i = 0; i < castles.Length; i++)
        {
            castles[i].Init();
        }
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
            new CountDownTimer(musicOffset, true).Stopped += () =>
            {
                source.Play();
            };
            Started?.Invoke();
        }
        
        if (isMainMusicStarted)
        {
            time += Time.deltaTime;
            MusicData.Update(time);
        }
    }
}
