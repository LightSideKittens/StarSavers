using System;
using System.Collections.Generic;
using Battle;
using Battle.Data;
using Battle.Data.GameProperty;
using BeatRoyale;
using Core.Server;
using Core.SingleService;
using DG.Tweening;
using GameCore.Battle.Data.Components;
using GameCore.Battle.Data.Components.HitBox;
using Sirenix.Serialization;
using UnityEngine;
using static SoundventTypes;

namespace GameCore.Battle.Data
{
    public class Tower : BaseEntity
    {
        public static event Action<Transform> Destroyed;
        public static HashSet<Transform> Towers { get; } = new();
        private static ShortNoteListener[] listeners;
        protected override IEnumerable<string> Entities => GameScopes.HeroesNames;
        
        [SerializeField] private float bulletFlyDuration = 0.4f;
        [SerializeField] private ParticleSystem deathFx;
        [SerializeField] private GameObject bulletPrefab;
        
        [OdinSerialize] private HitBoxComponent hitBoxComponent = new ColiderHitBoxComponent();
        [SerializeField] private HealthComponent healthComponent;
        [OdinSerialize] private FindTargetComponent findTargetComponent;
        private ShortNoteListener currentListener;
        private float damage;
        private int currentListenerIndex;

        private void Awake()
        {
            enabled = false;
            MusicController.EnableOnStart.Add(this);
            ServiceManager.Destroyed += OnWorldDestroy;
        }

        private void Start()
        {
            base.Init(User.Id);
            listeners ??= new[]
            {
                ShortNoteListener.Listen(ShortIII, -bulletFlyDuration),
                ShortNoteListener.Listen(ShortII, -bulletFlyDuration),
                ShortNoteListener.Listen(ShortI, -bulletFlyDuration)
            };
            
            listeners[2].Started += OnSoundvent;
            listeners[1].Started += OnSoundvent;
            listeners[0].Started += OnSoundvent;
            
            damage = GetProperties(transform)[nameof(DamageGP)].Value;
            currentListenerIndex = Towers.Count;
            currentListener = listeners[currentListenerIndex];
            currentListener.Started += Shoot;
            
            Towers.Add(transform);
            
            hitBoxComponent.Init(transform);
            findTargetComponent.Init(transform, false);
            healthComponent.Init(transform, false);
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
                    new CountDownTimer(0.35f).Stopped += () => Destroy(bullet);
                    
                    if (target.TryGet(out HealthComponent health))
                    {
                        health.TakeDamage(damage);
                        Instantiate(deathFx, findTargetComponent.target.position, Quaternion.identity);
                    }
                });
            }
        }

        private void OnWorldDestroy()
        {
            listeners = null;
        }
        
        protected override void OnDestroy()
        {
            base.OnDestroy();
            healthComponent.OnDestroy();
            hitBoxComponent.OnDestroy();
            Towers.Remove(transform);
            currentListener.Started -= Shoot;

            if (listeners != null)
            {
                listeners[2].Started -= OnSoundvent;
                listeners[1].Started -= OnSoundvent;
                listeners[0].Started -= OnSoundvent;
            }

            Destroyed?.Invoke(transform);
        }
    }
}