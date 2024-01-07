using System.Collections.Generic;
using Battle.Data;
using LSCore;
using UnityEngine;
using UnitsByTransform = Battle.ObjectsByTransfroms<Battle.Data.Unit>;

namespace Battle
{
    public abstract class BasePlayerWorld<T> : SingleService<T> where T : BasePlayerWorld<T>
    {
        private static HashSet<Unit> activeUnits;
        public static int UnitCount => activeUnits.Count;
        public static IEnumerable<Unit> ActiveUnits => activeUnits;
        
        private string userId;
        protected bool IsOpponent { get; private set; }

        protected string UserId
        {
            get => userId;
            set
            {
                userId = value;
                IsOpponent = value == "Opponent";
                activeUnits ??= new HashSet<Unit>();
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
            Debug.Log($"Stopped {typeof(T)}");
            Instance.enabled = false;
            Instance.OnStop();
        }
        
        protected virtual void OnBegin(){}
        protected virtual void OnStop(){}

        protected override void DeInit()
        {
            foreach (var unit in activeUnits)
            {
                unit.Destroy();
            }
            
            activeUnits.Clear();
            Unit.DestroyAllPools();
        }

        protected OnOffPool<Unit> CreatePool(Unit prefab)
        {
            var pool = Unit.CreatePool(prefab);
            pool.Created += InitUnit;
            pool.Got += OnUnitGot;
            pool.Released += OnUnitReleased;
            pool.Destroyed += OnUnitReleased;
            return pool;
        }

        private void InitUnit(Unit unit)
        {
            unit.Init(UserId);
        }

        private static void OnUnitGot(Unit unit) => activeUnits.Add(unit);
        private static void OnUnitReleased(Unit unit) => activeUnits.Remove(unit);

        private void Update()
        {
            foreach (var unit in activeUnits)
            {
                unit.Run();
            }
        }
        
        private void FixedUpdate()
        {
            foreach (var unit in activeUnits)
            {
                unit.FixedRun();
            }
        }
    }
}