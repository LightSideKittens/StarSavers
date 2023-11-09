using System.Collections.Generic;
using LSCore;

namespace Battle.Data
{
    public partial class Unit
    {
        private static readonly Dictionary<Id, OnOffPool<Unit>> pools = new();

        public static Unit Create(Unit prefab)
        {
            if (pools.TryGetValue(prefab.Id, out var pool)) return pool.Get();
            
            pool = CreatePool(prefab);
            pools.Add(prefab.Id, pool);

            return pool.Get();
        }

        public static void Destroy(Unit unit) => pools[unit.Id].Release(unit);
        public static OnOffPool<Unit> CreatePool(Unit prefab, int capacity = 10)
        {
            var pool = new OnOffPool<Unit>(prefab, capacity);
            pool.Got += OnGot;
            pool.Released += OnReleased;
            pool.Destroyed += OnDestroyed;
            pools.Add(prefab.Id, pool);
            return pool;
        }

        public static void ClearPool(Id id) => pools[id].Clear();
        
        private static void OnGot(Unit unit)
        {
            unit.Reset();
            unit.Enable();
        }
        
        private static void OnReleased(Unit unit) => unit.Disable();
        private static void OnDestroyed(Unit unit) => unit.Destroy();
    }
}