using System;
using System.Collections.Generic;
using Battle;
using LSCore.Extensions.Unity;
using UnityEngine;

namespace LSCore.BattleModule
{
    public class GroupUnitSpawner : MonoBehaviour
    {
        [Serializable]
        private struct Data
        {
            public Id id;
            public Vector2 position;
        }

        [SerializeField] private Data[] data;
        
        public void Spawn()
        {
            Vector2 position = BattleWorld.CameraRect.RandomPointAroundRect(2);
            for (int i = 0; i < data.Length; i++)
            {
                var d = data[i];
                var unit = OpponentWorld.Spawn(d.id);
                unit.transform.position = position + d.position;
            }
        }
    }
}