using System;
using System.Collections.Generic;
using Battle.Data;
using LSCore;
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

        [SerializeField] private CameraCollider2DTrigger cameraTrigger;
        
        [NonSerialized] private LevelsManager enemies;

        public static LevelsManager Enemies
        {
            get => Instance.enemies;
            set => Instance.enemies = value;
        }

        public static event Action<Unit> Killed; 
        private Dictionary<Id, OnOffPool<Unit>> pools = new();
        
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
        }

        public static Unit Spawn(Id id) => Instance.Get(id);
        
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
            unit.GetComp<BaseHealthComp>().Killed += () => {Killed?.Invoke(unit);};
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

        public static int GetFullBossHealth(RaidConfig.BossData bossData)
        {
            return Instance.Internal_GetFullBossHealth(bossData);
        }

        private int Internal_GetFullBossHealth(RaidConfig.BossData bossData)
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
        
        public static int GetBossHealth(RaidConfig.BossData bossData)
        {
            return Instance.Internal_GetBossHealth(bossData);
        }

        private int Internal_GetBossHealth(RaidConfig.BossData bossData)
        {
            var health = Internal_GetFullBossHealth(bossData);

            for (int i = 0; i < bossData.fromTo.x; i++)
            {
                var id = bossData.stageIds[i];
                var unit = enemies.GetCurrentLevel<Unit>(id);
                unit.RegisterComps();
                health -= unit.transform.Get<BaseHealthComp>().Health;
            }

            return health;
        }

        public static void UnleashKraken(RaidConfig.BossData bossData, Action onBossDefeated)
        {
            Instance.Internal_UnleashKraken(bossData, onBossDefeated);
        }

        private void Internal_UnleashKraken(RaidConfig.BossData bossData, Action onBossDefeated)
        {
            int stageIndex = bossData.fromTo.x;
            
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
                    if (stageIndex >= bossData.fromTo.y)
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