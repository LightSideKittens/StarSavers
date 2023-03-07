using System;
using DG.Tweening;
using MusicEventSystem.Configs;
using UnityEngine;

public class MusicReactiveTest : MonoBehaviour
{
    public static Transform[] Towers { get; private set; }
    private static readonly int addColorFade = Shader.PropertyToID("_AddColorFade");
    public static event Action Started;
    private int count;
    private float time;
    private bool isMainMusicStarted;

    [SerializeField] private Transform[] towers;
    [SerializeField] private string musicName;
    [SerializeField] private float timeOffset;
    [SerializeField] private AudioSource source;
    [SerializeField] private SpriteRenderer map;
    private float mapExposition;

    [SerializeField] private GameObject[] fires;
    [SerializeField] private Vector3 targetScale = new Vector3(5, 5, 5);
    [SerializeField] private float targetScaleSpeed = 3f;

    private void Awake()
    {
        MusicData.MusicName = musicName;
        time += timeOffset;
        source.time += timeOffset;
        MusicData.SkipToTime(timeOffset);
        Towers = towers;
    }

    private void Start()
    {
        MusicData.ShortTrackData.GetTrack(SoundventTypes.ShortIV).Started += () =>
        {
            Time.timeScale = 1;

            new CountDownTimer(0.1f, true).Stopped += () =>
            {
                DOTween.Kill("Scale");
                DOTween.Kill(this);
                mapExposition = 0.4f;

                for (int i = 0; i < fires.Length; i++)
                {
                    fires[i].transform.DOScale(targetScale, 0.1f).SetId("Scale");
                }
            };
        };
    }

    public void Update()
    {
        map.material.SetFloat(addColorFade, mapExposition);
        mapExposition -= Time.deltaTime * 1.5f;

        if (mapExposition < 0)
        {
            mapExposition = 0;
        }
        
        for (int i = 0; i < fires.Length; i++)
        {
            if (fires[i].transform.localScale.x > 1)
            {
                fires[i].transform.localScale -= Vector3.one * (Time.deltaTime * targetScaleSpeed);
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
