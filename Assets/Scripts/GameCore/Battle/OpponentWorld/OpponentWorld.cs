using System.Collections;
using System.Collections.Generic;
using Battle.Data;
using GameCore.Battle.Data;
using LSCore.Extensions;
using LSCore.Extensions.Unity;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Battle
{
    public class OpponentWorld : BasePlayerWorld<OpponentWorld>
    {
#if UNITY_EDITOR
        private static IList<ValueDropdownItem<int>> Enemies => EntityMeta.GetGroupByName("Enemies").EntityValues;
#endif
        
        [SerializeField, ValueDropdown("Enemies")] private int[] enemyIds;
        [SerializeField] private Enemies enemies;
        [SerializeField] private Transform spawnPoint;
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
            cameraRect.position = cam.transform.position;
            for (int i = 0; i < Random.Range(10, 20); i++)
            {
                Internal_Spawn(Heroes.ByName[enemyIds.Random()], cameraRect.RandomPointAroundRect());
            }
        }

        protected override void OnStop()
        {
            
        }
    }
}