using System;
using LSCore;
using LSCore.LevelSystem;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Battle.Data
{
    public class Bullets : ValuesById<Bullets.Data>
    {
        [Serializable]
        public struct Data
        {
            public GameObject bulletPrefab;
            public GameObject explosionPrefab;
            public float bulletFlyDuration;
        }
        
        [SerializeField] [IdGroup] private LevelIdGroup group;
        
#if UNITY_EDITOR
        protected override void SetupDataSelector(ValueDropdownList<Entry> list)=> SetupByGroup(group, list);
#endif
    }
}