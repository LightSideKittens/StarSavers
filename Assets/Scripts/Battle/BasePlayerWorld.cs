using System.Collections.Generic;
using Battle.Data;
using LSCore;
using UnityEngine;
using UnitsByTransform = Battle.ObjectsByTransfroms<Battle.Data.Unit>;

namespace Battle
{
    public abstract class BasePlayerWorld<T> : SingleService<T> where T : BasePlayerWorld<T>
    {
        private static HashSet<Unit> units;
        public static int UnitCount => units.Count;
        public static IEnumerable<Unit> Units => units;
        
        private string userId;
        protected bool IsOpponent { get; private set; }

        protected string UserId
        {
            get => userId;
            set
            {
                userId = value;
                IsOpponent = value == "Opponent";
                units ??= new HashSet<Unit>();
            }
        }

        protected override void Init()
        {
            enabled = false;
        }

        public static void Begin()
        {
            Instance.enabled = true;
            Instance.OnBegin();
        }
        
        public static void Stop()
        {
            Debug.Log($"Stoped {typeof(T)}");
            Instance.enabled = false;
            Instance.OnStop();
        }
        
        protected virtual void OnBegin(){}
        protected virtual void OnStop(){}

        protected override void DeInit()
        {
            foreach (var unit in units)
            {
                unit.Destroy();
            }
            
            units.Clear();
            Unit.DestroyAllPools();
        }

        protected OnOffPool<Unit> CreatePool(Unit prefab)
        {
            var pool = Unit.CreatePool(prefab);
            pool.Created += InitUnit;
            pool.Destroyed += OnUnitDestroyed;
            return pool;
        }

        private static void OnUnitDestroyed(Unit unit)
        {
            units.Remove(unit);
        }

        private void InitUnit(Unit unit)
        {
            unit.Init(UserId);
            units.Add(unit);
        }

        private void Update()
        {
            foreach (var unit in units)
            {
                unit.Run();
            }
        }
        
        private void FixedUpdate()
        {
            foreach (var unit in units)
            {
                unit.FixedRun();
            }
        }
    }
}