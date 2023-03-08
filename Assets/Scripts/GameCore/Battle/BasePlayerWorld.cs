using System.Collections.Generic;
using Core.SingleService;
using GameCore.Battle.Data;
using UnityEngine;

namespace Battle
{
    public abstract class BasePlayerWorld<T> : SingleService<T> where T : BasePlayerWorld<T>
    {
        protected abstract bool IsOpponent { get; }

        private IEnumerable<Unit> Units
        {
            get
            {
                foreach (var unitByTransform in Unit.ByTransform)
                {
                    var unit = unitByTransform.Value;

                    if (unit.IsOpponent == IsOpponent)
                    {
                        yield return unit;
                    }
                }
            }
        }

        protected override void Awake()
        {
            base.Awake();
            enabled = false;
            MusicReactiveTest.Started += () => enabled = true;
        }

        public static void Spawn(Unit prefab, Vector2 position) => Instance.Internal_Spawn(prefab, position);

        protected void Internal_Spawn(Unit prefab, Vector2 position)
        {
            var unit = Instantiate(prefab, position, Quaternion.identity);
            InitUnit(unit);
        }
        
        private void InitUnit(Unit unit)
        {
            unit.Init(IsOpponent);
        }

        private void Update()
        {
            foreach (var unit in Units)
            {
                unit.Run();
            }
        }
        
        private void FixedUpdate()
        {
            foreach (var unit in Units)
            {
                unit.FixedRun();
            }
        }
    }
}