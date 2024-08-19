using System;
using System.Collections.Generic;
using Battle.Data;
using DG.Tweening;
using LSCore;
using LSCore.Async;
using LSCore.BattleModule;
using LSCore.Extensions.Unity;
using LSCore.LevelSystem;
using UnityEngine;

namespace Battle
{
    public partial class OpponentWorld : BasePlayerWorld<OpponentWorld>
    {
        [Serializable]
        public class MoveBehindCamera : CameraCollider2DTrigger.Handler
        {
            public override void Invoke(Collider2D value)
            {
                value.transform.position = BattleWorld.CameraRect.RandomPointAroundRect(2);
            }
        }
        
        private const int MaxEnemyCount = 100;
        [SerializeField] private LevelsManager enemies;
        [SerializeField] private CameraCollider2DTrigger cameraTrigger;
        
        private Dictionary<Id, OnOffPool<Unit>> pools = new();
        private Tween spawnLoopTween;
        
        public override string UserId => "Opponent";
        public override string TeamId => "Opponent's Team";
        
        protected override void OnBegin()
        {
            AddDebugData();
        }

        private OnOffPool<Unit> CreatePoolById(Id id)
        {
            var unit = enemies.GetCurrentLevel<Unit>(id);
            var pool = CreatePool(unit);
            pool.Got += OnGot;
            pool.Released += OnReleased;
            SubscribeOnChange(pool);
            return pool;
        }

        protected override void OnStop()
        {
            RemoveDebugData();
            spawnLoopTween.Kill();
        }

        private void Spawn()
        {
            if (UnitCount < MaxEnemyCount)
            {
                Get(BattleWorld.GetEnemyId());
            }
            
            spawnLoopTween = Wait.Delay(BattleWorld.GetSpawnFrequency(), Spawn);
        }

        private Unit Get(Id id)
        {
            if (!pools.TryGetValue(id, out var pool))
            {
                pool = CreatePoolById(id);
                pools.Add(id, pool);
            }
            
            return pool.Get();
        }

        protected override void InitUnit(Unit unit)
        {
            UpdateUnitPosition(unit);
            base.InitUnit(unit);
        }

        private void OnGot(Unit unit)
        {
            UpdateUnitPosition(unit);
            cameraTrigger.Register(unit.GetComponent<Collider2D>());
        }
        
        private void OnReleased(Unit unit)
        {
            cameraTrigger.Unregister(unit.GetComponent<Collider2D>());
        }

        private void UpdateUnitPosition(Unit unit)
        {
            unit.transform.position = BattleWorld.CameraRect.RandomPointAroundRect(2);
        }
        
        public static void Continue() => Instance.Spawn();
        public static void Pause() => Instance.spawnLoopTween.Kill();

        private void OnDrawGizmosSelected()
        {
            var oldColor = Gizmos.color;
            Gizmos.color = new Color(0f, 1f, 0f, 0.49f);
            Gizmos.DrawCube(BattleWorld.CameraRect.center, BattleWorld.CameraRect.size);
            Gizmos.color = oldColor;
        }

        static partial void SubscribeOnChange(OnOffPool<Unit> pool);
        static partial void AddDebugData();
        static partial void RemoveDebugData();

        public static int GetBossHealth(RaidConfig.BossData bossData)
        {
            return Instance.Internal_GetBossHealth(bossData);
        }

        private int Internal_GetBossHealth(RaidConfig.BossData bossData)
        {
            var health = 0;

            foreach (var id in bossData.stageIds)
            {
                var unit = enemies.GetCurrentLevel<Unit>(id);
                unit.RegisterComps();
                health += unit.transform.Get<BaseHealthComp>().Health;
            }

            return health;
        }

        public static void UnleashKraken(RaidConfig.BossData bossData, Action onBossDefeated)
        {
            Instance.Internal_UnleashKraken(bossData, onBossDefeated);
        }

        private void Internal_UnleashKraken(RaidConfig.BossData bossData, Action onBossDefeated)
        {
            int stageIndex = 0;
            Unleash();

            return;

            void Unleash()
            {
                var unit = Get(bossData.stageIds[stageIndex]);
                unit.Released += OnReleased;
                return;

                void OnReleased()
                {
                    stageIndex++;
                    if (stageIndex >= bossData.stageIds.Length)
                    {
                        onBossDefeated();
                        return;
                    }
                    Unleash();
                }
            }
        }
    }
}