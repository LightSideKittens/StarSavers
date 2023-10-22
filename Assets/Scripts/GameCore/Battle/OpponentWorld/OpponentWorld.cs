using System.Linq;
using DG.Tweening;
using GameCore.Battle;
using GameCore.Battle.Data;
using LSCore.Async;
using LSCore.Extensions;
using LSCore.Extensions.Unity;
using UnityEngine;
using UnityEngine.Pool;

namespace Battle
{
    public class OpponentWorld : BasePlayerWorld<OpponentWorld>
    {
        [SerializeField, EntityId("Enemies")] private int[] enemyIds;
        [SerializeField] private Enemies enemies;
        private Camera cam;
        private Tween spawnLoopTween;
        private static Rect cameraRect;
        
        public static IObjectPool<Transform> Pool { get; private set; }
                
        private Transform CreatePooledItem()
        {
            cameraRect.center = cam.transform.position;
            return Spawn(Heroes.ByName[enemyIds.Random()]).transform;
        }
        
        private static void OnTakeFromPool(Transform transform)
        {
            var unit = transform.Get<Unit>();
            transform.position = cameraRect.RandomPointAroundRect();
            unit.Reset();
            unit.Enable();
        }
        
        private static void OnReturnedToPool(Transform unit)
        {
            unit.Get<Unit>().Disable();
        }

        private static void OnDestroyPoolObject(Transform unit)
        {
            unit.Get<Unit>().Destroy();
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
            var units = Unit.All[UserId].Values.ToList();
            
            foreach (var unit in units)
            {
                unit.Destroy();
            }
            
            Pool.Clear();
            spawnLoopTween.Kill();
        }

        private void Spawn() => Pool.Get();

        private void OnDrawGizmosSelected()
        {
            var oldColor = Gizmos.color;
            Gizmos.color = new Color(0f, 1f, 0f, 0.49f);
            Gizmos.DrawCube(cameraRect.center, cameraRect.size);
            Gizmos.color = oldColor;
        }
    }
}