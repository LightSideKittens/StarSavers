using System.Diagnostics;
using System.Linq;
using DG.Tweening;
using Battle.Data;
using LSCore.Async;
using LSCore.Extensions;
using LSCore.Extensions.Unity;
using UnityEngine;
using UnityEngine.Pool;

namespace Battle
{
    public partial class OpponentWorld : BasePlayerWorld<OpponentWorld>
    {
        private const int MaxEnemyCount = 100;
        [SerializeField, Id("Enemies")] private Id[] enemyIds;
        [SerializeField] private Enemies enemies;
        private Camera cam;
        private Tween spawnLoopTween;
        private static Rect cameraRect;
        
        public static IObjectPool<Transform> Pool { get; private set; }
                
        private Transform CreatePooledItem()
        {
            cameraRect.center = cam.transform.position;
            return Spawn(Heroes.ByName[enemyIds.Random()]).transform;;
        }
        
        private static void OnTakeFromPool(Transform transform)
        {
            var unit = transform.Get<Unit>();
            transform.position = cameraRect.RandomPointAroundRect(5);
            unit.Reset();
            unit.Enable();
            OnChange();
        }
        
        private static void OnReturnedToPool(Transform unit)
        {
            unit.Get<Unit>().Disable();
            OnChange();
        }

        private static void OnDestroyPoolObject(Transform unit)
        {
            unit.Get<Unit>().Destroy();
            OnChange();
        }
        
        [Conditional("DEBUG")]
        public static void OnChange()
        {
#if DEBUG
            DebugData.OnChange();   
#endif
        }
        
        protected override void OnBegin()
        {
            UserId = "Opponent";
            enemies.Init();
            cam = Camera.main;
            cameraRect = cam.GetRect();
            Pool = new ObjectPool<Transform>(Instance.CreatePooledItem, OnTakeFromPool, OnReturnedToPool, OnDestroyPoolObject, true, 100, 500);
            Spawn();
            spawnLoopTween = Wait.InfinityLoop(0.2f, Spawn);
        }
        
        protected override void OnStop()
        {
            var units = Unit.ByWorld[UserId].Values.ToList();
            
            foreach (var unit in units)
            {
                unit.Destroy();
            }
            
            Pool.Clear();
            spawnLoopTween.Kill();
        }

        private void Spawn()
        {
            if (UnitCount < MaxEnemyCount)
            {
                Pool.Get();
            }
        }

        private void OnDrawGizmosSelected()
        {
            var oldColor = Gizmos.color;
            Gizmos.color = new Color(0f, 1f, 0f, 0.49f);
            Gizmos.DrawCube(cameraRect.center, cameraRect.size);
            Gizmos.color = oldColor;
        }
    }
}