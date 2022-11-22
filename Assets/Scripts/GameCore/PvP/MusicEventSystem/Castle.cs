using System;
using System.Collections.Generic;
using DG.Tweening;
using JetBrains.Annotations;
using MusicEventSystem.Configs;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

public class Castle : MonoBehaviour
{
    private Queue<GameObject> enemies = new ();
    [SerializeField] protected ParticleSystem firstSoundTypeFX;
    [SerializeField] protected ParticleSystem secondSoundTypeFX;

    [SerializeField] private float enemyMoveDuration = 2f;
    [SerializeField] private float bulletFlyDuration = 0.1f;

    [ValueDropdown("Types")]
    [SerializeField]
    private string firstSoundType;
    
    [ValueDropdown("Types")]
    [SerializeField]
    private string secondSoundType;

    [UsedImplicitly]
    private static IEnumerable<string> Types => SoundTypes.Types;

    public void Init()
    {
        InitAllTracks<MainMusicData>(InitTrack);
        InitAllTracks<EnemyMusicData>(InitEnemyTrack);
    }

    private void InitAllTracks<T>(Action<TrackData, string> action) where T : BaseMusicData<T>, new()
    {
        action(BaseMusicData<T>.GetTrack(firstSoundType), firstSoundType);
        action(BaseMusicData<T>.GetTrack(secondSoundType), secondSoundType);
    }

    private void InitTrack(TrackData trackData, string soundType)
    {
        trackData.Started += () => DestroyEnemy(soundType);
    }

    private void InitEnemyTrack(TrackData trackData, string soundType)
    {
        trackData.Started += () => CreateEnemy(soundType);
    }
    
    public void CreateEnemy(string soundType)
    {
        var enemy = Instantiate(EnemyBulletPairs.Pairs[soundType].enemy, new Vector3(Random.Range(-2.6f, 2.6f), 6, 0), Quaternion.identity);
        enemy.transform.DOMove(enemy.transform.position - new Vector3(0, 12, 0), enemyMoveDuration);
        enemies.Enqueue(enemy);
    }

    public void DestroyEnemy(string soundType)
    {
        var enemy = enemies.Dequeue();
        OnDestroyEnemy(soundType, enemy);
    }

    protected virtual void OnDestroyEnemy(string soundType, GameObject enemy)
    {
        var bullet = Instantiate(EnemyBulletPairs.Pairs[soundType].bullet, transform.position, Quaternion.identity);
        bullet.transform.DOMove(enemy.transform.position, bulletFlyDuration).OnComplete(() =>
        {
            Instantiate(GetFX(soundType), enemy.transform.position, Quaternion.identity);
            Destroy(bullet);
            Destroy(enemy.gameObject);
        });
    }

    protected ParticleSystem GetFX(string soundType)
    {
        return soundType == firstSoundType ? firstSoundTypeFX : secondSoundTypeFX;
    }
}
