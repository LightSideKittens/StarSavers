using System;
using System.Collections.Generic;
using BeatRoyale;
using GameCore.Battle.Data.Components;
using GameCore.Battle.Data.Components.HitBox;
using Sirenix.Serialization;
using UnityEngine;

namespace GameCore.Battle.Data
{
    public class Cannon : BaseEntity
    {
        public static event Action<Transform> Destroyed;
        [OdinSerialize] private HitBoxComponent hitBoxComponent = new ColiderHitBoxComponent();
        [OdinSerialize] private FindTargetComponent findTargetComponent = new();
        [SerializeField] private CannonAttackComponent attackComponent;
        [SerializeField] private HealthComponent healthComponent;
        public static HashSet<Transform> Cannons { get; } = new();

        private void Awake()
        {
            enabled = false;
            MusicController.EnableOnStart.Add(this);
        }

        private void Start()
        {
            Cannons.Add(transform);
            base.Init(MatchData.OpponentUserId);
            hitBoxComponent.Init(transform);
            findTargetComponent.Init(transform, true);
            healthComponent.Init(transform, true);
            attackComponent.Init(transform, findTargetComponent);
        }

        public void Update()
        {
            findTargetComponent.Find();
            attackComponent.Update();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            hitBoxComponent.OnDestroy();
            healthComponent.OnDestroy();
            attackComponent.OnDestroy();
            Cannons.Remove(transform);
            Destroyed?.Invoke(transform);
        }
    }
}