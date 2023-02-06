using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Battle.ConfigsSO
{
    public abstract class BaseBulletsData<T> : ScriptableObject where T : BaseBulletsData<T>
    {
        [Serializable]
        public struct Data
        {
            [ValueDropdown(nameof(Types))]
            public string soundType;
            public GameObject prefab;
            [Min(10f)] public float damage;
            
            private static IEnumerable<string> Types => SoundventTypes.Sounds;
        }

        [SerializeField] private List<Data> bullets;
        private static readonly Dictionary<string, Data> bulletsSet = new ();

        public static Data GetBullet(string soundType)
        {
            return bulletsSet[soundType];
        }
        
        public void Init()
        {
            bulletsSet.Clear();
        
            for (int i = 0; i < bullets.Count; i++)
            {
                var pair = bullets[i];
                bulletsSet.Add(pair.soundType, pair);
            }
        }
    }
}