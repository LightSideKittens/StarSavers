using System;
using System.Collections.Generic;
using Core.SingleService;
using DG.Tweening;
using MusicEventSystem.Configs;
using UnityEngine;

public class MusicController : SingleService<MusicController>
{
    public static int MusicOffset => 1;
    public static List<MonoBehaviour> EnableOnStart { get; } = new();
    private static readonly int addColorFade = Shader.PropertyToID("_AddColorFade");
    private int count;
    private float time;
    private bool isMainMusicStarted;
    
    [SerializeField] private string musicName;
    [SerializeField] private float timeOffset;
    [SerializeField] private AudioSource source;
    [SerializeField] private SpriteRenderer map;
    private float mapExposition;

    [SerializeField] private GameObject[] fires;
    [SerializeField] private Vector3 targetScale = new Vector3(5, 5, 5);
    [SerializeField] private float targetScaleSpeed = 3f;

    public static void Begin()
    {
        Resume();
    }

    public static void Stop()
    {
        Pause();
        EnableOnStart.Clear();
    }

    public static void Pause()
    {
        Instance.enabled = false;
    }
    
    public static void Resume()
    {
        Instance.enabled = true;
    }
    
    protected override void Awake()
    {
        base.Awake();
        MusicData.MusicName = musicName;
        time += timeOffset;
        source.time += timeOffset;
        MusicData.SkipToTime(timeOffset);
        Pause();
    }

    private void Start()
    {
        MusicData.ShortTrackData.GetTrack(SoundventTypes.ShortIII).Started += () =>
        {
            new CountDownTimer(MusicOffset + 0.1f, true).Stopped += () =>
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
            new CountDownTimer(MusicOffset, true).Stopped += () =>
            {
                source.Play();
            };

            for (int i = 0; i < EnableOnStart.Count; i++)
            {
                EnableOnStart[i].enabled = true;
            }
        }
        
        if (isMainMusicStarted)
        {
            time += Time.deltaTime;
            MusicData.Update(time);
        }
    }
}
