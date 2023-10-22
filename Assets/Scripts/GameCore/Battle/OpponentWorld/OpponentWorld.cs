using System;
using System.Collections;
using GameCore.Battle.Data;
using LSCore.Extensions;
using LSCore.Extensions.Unity;
using UnityEngine;

namespace Battle
{
    public class OpponentWorld : BasePlayerWorld<OpponentWorld>
    {
        [SerializeField, EntityId("Enemies")] private int[] enemyIds;
        [SerializeField] private Enemies enemies;
        private Camera cam;
        private Rect cameraRect;
        
        protected override void Init()
        {
            UserId = "Opponent";
            enemies.Init();
            cam = Camera.main;
            cameraRect = cam.GetRect();
        }

        private void OnEnable()
        {
            StartCoroutine(SpawnOpponents());
        }

        private IEnumerator SpawnOpponents()
        {
            while (enabled)
            {
                Spawn();
                yield return new WaitForSeconds(1);
            }
        }

        private void Spawn()
        {
            cameraRect.center = cam.transform.position;
            Internal_Spawn(Heroes.ByName[enemyIds.Random()], cameraRect.RandomPointAroundRect());
        }

        private void OnDrawGizmosSelected()
        {
            var oldColor = Gizmos.color;
            Gizmos.color = new Color(0f, 1f, 0f, 0.49f);
            Gizmos.DrawCube(cameraRect.center, cameraRect.size);
            Gizmos.color = oldColor;
        }

        protected override void OnStop()
        {
            
        }
    }
}