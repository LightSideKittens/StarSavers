using System;
using System.Collections.Generic;
using Battle;
using Battle.ConfigsSO;
using JetBrains.Annotations;
using MusicEventSystem.Configs;
using PathCreation;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

public class Castle : MonoBehaviour
{
    private static Dictionary<string, List<Enemy>> allEnemies = new ();
    private static int allEnemyForShootIndex;
    
    private List<Enemy> enemies = new ();
    [SerializeField] protected ParticleSystem firstSoundTypeFX;
    [SerializeField] protected ParticleSystem secondSoundTypeFX;

    [SerializeField] private float enemyMoveDuration = 2f;
    [SerializeField] protected float bulletFlyDuration = 0.1f;

    [ValueDropdown(nameof(Types))]
    [SerializeField]
    private string firstSoundType;
    
    [ValueDropdown(nameof(Types))]
    [SerializeField]
    private string secondSoundType;
    
    [ValueDropdown(nameof(Types))]
    [SerializeField]
    private string[] enemiesTypesByPriority;
    
    protected string currentSoundType;
    private int enemyForShootIndex;
    private bool needFx;

    [UsedImplicitly]
    private static IEnumerable<string> Types => SoundventTypes.Sounds;

    public void Init()
    {
        MusicReactiveTest.Started += OnGameStarted;
        if (allEnemies.ContainsKey("Boss") == false)
        {
            allEnemies.Add("Boss", new List<Enemy>());
        }
        
        allEnemies.Add(firstSoundType, new List<Enemy>());
        allEnemies.Add(secondSoundType, new List<Enemy>());
    }

    private void InitTrack(LongNoteTrackData longNoteTrackData, string soundType)
    {
        longNoteTrackData.Started += OnStarted;
        
        void OnStarted()
        {
            needFx = true;
            currentSoundType = soundType;
            DamageEnemy<ActiveBulletsData>();
        }
    }

    private void InitEnemyTrack(LongNoteTrackData longNoteTrackData, string soundType)
    {
        longNoteTrackData.Started += () => CreateEnemy<Enemy>(soundType);
    }

    private void OnGameStarted()
    {
        //EnemyMusicData.BPMReached += OnBPMReached;
        //CreateEnemy<Boss>("Boss");
        MusicReactiveTest.Started -= OnGameStarted;
        
        void OnBPMReached()
        {
            needFx = false;
            currentSoundType = firstSoundType;
            DamageEnemy<PassiveBulletsData>();
        }
    }
    
    public void CreateEnemy<T>(string soundType) where T : Enemy, new()
    {
        var enemy = new T();
        enemy.Init(soundType);
        enemy.Destroyed += OnDestroyed;
        enemy.WillDestroyed += OnWillDestroyed;

        enemy.StartMove(enemyMoveDuration);
        enemies.Add(enemy);
        allEnemies[soundType].Add(enemy);
        
        void OnDestroyed()
        {
            enemyForShootIndex--;
            allEnemyForShootIndex--;
            enemies.Remove(enemy);
            allEnemies[soundType].Remove(enemy);
            enemy.Destroyed -= OnDestroyed;
        }

        void OnWillDestroyed()
        {
            enemyForShootIndex++;
            allEnemyForShootIndex++;
            enemy.WillDestroyed -= OnWillDestroyed;
        }
    }

    private void DamageEnemy<T>() where T : BaseBulletsData<T>, new()
    {
        if (TryGetEnemy(out var enemy))
        {
            if (firstSoundType != SoundventTypes.LongII)
            {
                OnDamageEnemy<T>(enemy);
            }
            else if (typeof(T) == typeof(PassiveBulletsData))
            {
                Shoot<T>(enemy);
            }
            else
            {
                OnDamageEnemy<T>(enemy);
            }
        }
    }

    private bool TryGetEnemy(List<Enemy> enemies, int enemyIndex, out Enemy enemy)
    {
        if (enemies.Count > enemyIndex)
        {
            enemy = enemies[enemyIndex];
            return true;
        }

        enemy = null;
        return false;
    }

    private bool TryGetEnemy(out Enemy enemy)
    {
        var isExist = TryGetEnemy(allEnemies["Boss"], 0, out enemy);
        isExist |= TryGetEnemy(enemies, enemyForShootIndex, out enemy);

        var i = 0;
        while (!isExist && i < enemiesTypesByPriority.Length)
        {
            isExist = TryGetEnemy(allEnemies[enemiesTypesByPriority[i]], 0, out enemy);
            i++;
        }

        return isExist;
    }

    protected virtual void OnDamageEnemy<T>(Enemy enemy) where T : BaseBulletsData<T>, new()
    {
        Shoot<T>(enemy);
    }

    private void Shoot<T>(Enemy enemy) where T : BaseBulletsData<T>, new()
    {
        var bullet = new Bullet<T>(this, needFx ? GetFX(currentSoundType) : null, currentSoundType);
        bullet.Shoot(enemy, bulletFlyDuration);
    }

    protected ParticleSystem GetFX(string soundType)
    {
        return soundType == firstSoundType ? firstSoundTypeFX : secondSoundTypeFX;
    }
}
