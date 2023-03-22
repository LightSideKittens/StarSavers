using System;
using System.Collections.Generic;
using Battle.Data;
using Battle.Data.GameProperty;
using BeatRoyale;
using DG.Tweening;
using GameCore.Battle.Data.Components;
using GameCore.Battle.Data.Components.HitBox;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using static SoundventTypes;

namespace GameCore.Battle.Data
{
    public class Tower : SerializedMonoBehaviour
    {
        public static event Action<Transform> Destroyed;
        public static HashSet<Transform> Towers { get; } = new();
        private static ShortNoteListener[] listeners;
        private static IEnumerable<string> HeroesNames => GameScopes.HeroesNames;

        [SerializeField, ValueDropdown(nameof(HeroesNames))] private string entityName;
        [SerializeField] private float bulletFlyDuration = 0.4f;
        [SerializeField] private ParticleSystem deathFx;
        [SerializeField] private GameObject bulletPrefab;
        
        [OdinSerialize] private HitBoxComponent hitBoxComponent = new ColiderHitBoxComponent();
        [SerializeField] private HealthComponent healthComponent;
        [OdinSerialize] private FindTargetComponent findTargetComponent;
        private ShortNoteListener currentListener;
        private float damage;
        private int currentListenerIndex;

        private void Start()
        {
            listeners ??= new[]
            {
                ShortNoteListener.Listen(ShortIII, -bulletFlyDuration),
                ShortNoteListener.Listen(ShortII, -bulletFlyDuration),
                ShortNoteListener.Listen(ShortI, -bulletFlyDuration)
            };
            
            listeners[2].Started += OnSoundvent;
            listeners[1].Started += OnSoundvent;
            listeners[0].Started += OnSoundvent;
            
            damage = EntitiesProperties.ByName[entityName][nameof(DamageGP)].Value;
            currentListenerIndex = Towers.Count;
            currentListener = listeners[currentListenerIndex];
            currentListener.Started += Shoot;
            
            Towers.Add(transform);
            
            hitBoxComponent.Init(gameObject);
            findTargetComponent.Init(gameObject, false);
            healthComponent.Init(entityName, gameObject, false);
        }

        private void OnSoundvent()
        {
            currentListenerIndex++;
            currentListenerIndex %= Towers.Count;
            currentListener.Started -= Shoot;
            currentListener = listeners[currentListenerIndex];
            currentListener.Started += Shoot;
        }

        private void Shoot()
        {
            if (findTargetComponent.Find())
            {
                var bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
                var target = findTargetComponent.target;
                bullet.transform.DOMove(target.position, 0.4f).SetEase(Ease.InExpo).OnComplete(() =>
                {
                    if (target.TryGet(out HealthComponent health))
                    {
                        new CountDownTimer(0.35f).Stopped += () => Destroy(bullet);
                        health.TakeDamage(damage);
                        Instantiate(deathFx, findTargetComponent.target.position, Quaternion.identity);
                    }
                });
            }
        }

        private void OnDestroy()
        {
            healthComponent.OnDestroy();
            hitBoxComponent.OnDestroy();
            Towers.Remove(transform);
            currentListener.Started -= Shoot;
            listeners[2].Started -= OnSoundvent;
            listeners[1].Started -= OnSoundvent;
            listeners[0].Started -= OnSoundvent;
            Destroyed?.Invoke(transform);
        }
    }
}