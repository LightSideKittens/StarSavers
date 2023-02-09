using Battle;
using Battle.Data;
using DG.Tweening;
using MusicEventSystem.Configs;
using UnityEngine;

public class BassBullet : MonoBehaviour
{
    public LineRenderer myLineRenderer;
    public int points;
    public float amplitude = 1;
    public float frequency = 1;
    public Vector2 xLimits = new Vector2(0,1);
    public float shakeStrength = 1;
    public int shakeVibrato = 30;
    public float movementSpeed = 1;
    private Transform target;
    [SerializeField] private ParticleSystem startFx;
    private ParticleSystem destroyFX;
    private float attackFlyDuration;
    private float damage;
    private Enemy enemy;

    public static BassBullet Create<T>(Castle castle, string soundType, float attackFlyDuration, Enemy enemy, ParticleSystem destroyFX) where T : BaseBulletsData<T>, new()
    {
        var data = BaseBulletsData<T>.GetBullet(soundType);
        var bullet = Instantiate(data.prefab, Vector3.back, Quaternion.identity).GetComponent<BassBullet>();
        bullet.damage = data.damage;
        bullet.attackFlyDuration = attackFlyDuration;
        bullet.destroyFX = destroyFX;
        bullet.Init(soundType, enemy);

        return bullet;
    }
    
    private void Awake()
    {
        myLineRenderer = GetComponent<LineRenderer>();
    }

    private void Init(string soundType, Enemy enemy)
    {
        enabled = false;
        this.enemy = enemy;
        var noteDurarion = MusicData.LongTrackData.GetTrack(soundType).CurrentLongNote.Duration;

        DOTween.To(() => 0f, OnFly, 1f, attackFlyDuration).SetEase(Ease.InCubic).SetId(enemy);
        var attackTimer = new CountDownTimer(attackFlyDuration, true).SetId(enemy);
        attackTimer.Stopped += OnFlyComplete; 

        DOTween.Kill("CameraShake");
        //target.DOScale(target.localScale * enemyScaleMultiplier, noteDurarion).SetId("TargetScale");
        Camera.main.DOShakeRotation(noteDurarion, shakeStrength, shakeVibrato).OnComplete(MusicReactiveTest.ResetCameraPosition).SetId("CameraShake");

        void OnFlyComplete()
        {
            enabled = true;
            Instantiate(startFx, target.position, Quaternion.identity, target);
            var noteTimer = new CountUpTimer(noteDurarion, true).SetId(this);
            noteTimer.Stopped += Destroy;
        }

        void OnFly(float time)  
        {
            var direction = target.position - transform.position;
            var distance = direction.magnitude;
            
            xLimits.y = distance * time;
            transform.right = direction;
            Draw();
        }
    }

    private void Destroy()
    {
        var startColor = myLineRenderer.startColor;
        startColor.a = 0;
        var endColor = myLineRenderer.endColor;
        endColor.a = 0;
        myLineRenderer.DOColor(new Color2(myLineRenderer.startColor, myLineRenderer.endColor), new Color2(startColor, endColor), 0.2f).OnComplete(() => Destroy(gameObject));
        TimerExtensions.Kill(this);
        TimerExtensions.Kill(enemy);
        DOTween.Kill(enemy);
        DOTween.Kill("TargetScale");
        DOTween.Kill("CameraShake");
        MusicReactiveTest.ResetCameraPosition();
    }
    
    private void Draw()
    {
        const float tau = 2 * Mathf.PI;
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
        if (target != null)
        {
            var direction = target.position - transform.position;
            var distance = direction.magnitude;

            xLimits.y = distance;
            transform.right = direction;
            var tempDamage = damage * (120 / 64f) * Time.deltaTime;
        }
        
        Draw();
    }
}