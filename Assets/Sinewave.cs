using System;
using DG.Tweening;
using MusicEventSystem.Configs;
using UnityEngine;
using static SoundTypes;

public class Sinewave : MonoBehaviour
{
    public LineRenderer myLineRenderer;
    public int points;
    public float amplitude = 1;
    public float frequency = 1;
    public Vector2 xLimits = new Vector2(0,1);
    public float shakeStrength = 1;
    public int shakeVibrato = 30;
    public float movementSpeed = 1;
    public float attackFlyDuration = 0.3f;
    public event Action NoteEnded;
    private Transform target;
    [SerializeField] private float enemyScaleMultiplier = 2;

    private void Awake()
    {
        myLineRenderer = GetComponent<LineRenderer>();
        enabled = false;
    }

    public void SetTarget(string soundType, Transform target)
    {
        this.target = target;

        var attackTimer = new CountUpTimer(attackFlyDuration, true);
        
        attackTimer.NormalizeUpdated += time =>
        {
            var direction = target.position - transform.position;
            var distance = direction.magnitude;
            
            xLimits.y = distance * time;
            transform.right = direction;
            Draw();
        };

        var noteDurarion = MainMusicData.GetTrack(soundType).CurrentNote.Duration;
        
        attackTimer.Stopped += () =>
        {
            target.DOScale(target.localScale * enemyScaleMultiplier, noteDurarion);
            Camera.main.DOShakeRotation(noteDurarion, shakeStrength, shakeVibrato);
            enabled = true;
        };

        var noteTimer = new CountUpTimer(noteDurarion, true);
        
        noteTimer.Stopped += () =>
        {
            NoteEnded?.Invoke();
            Destroy(gameObject);
        };
    }

    private void Draw()
    {
        const float tau = 2* Mathf.PI;
        var xStart = xLimits.x;
        var xFinish = xLimits.y;
 
        myLineRenderer.positionCount = points;
        
        for(int currentPoint = 0; currentPoint < points; currentPoint++)
        {
            var progress = (float)currentPoint/(points-1);
            var x = Mathf.Lerp(xStart,xFinish,progress);
            var y = amplitude*Mathf.Sin(tau*frequency*x + Time.timeSinceLevelLoad*movementSpeed);
            myLineRenderer.SetPosition(currentPoint, new Vector3(x,y,0));
        }
    }

    private void Update()
    {
        var direction = target.position - transform.position;
        var distance = direction.magnitude;

        xLimits.y = distance;
        transform.right = direction;
        
        Draw();
    }
}