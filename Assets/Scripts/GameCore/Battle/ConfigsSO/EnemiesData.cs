using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace Battle.ConfigsSO
{
    [CreateAssetMenu(fileName = "EnemiesData", menuName = "Battle/EnemiesData", order = 0)]
    public class EnemiesData : ScriptableObject
    {
        [Serializable]
        public struct Data
        {
            [ValueDropdown(nameof(Types))]
            public string soundType;
            public GameObject prefab;
            [ReferenceFrom(nameof(prefab))] 
            public TMP_Text text;
            [Min(100f)] public float health;
            public float damage;
            
            private static IEnumerable<string> Types => SoundventTypes.Sounds;
        }

        [SerializeField] private List<Data> enemies;
        private static Dictionary<string, Data> enemiesSet = new ();

        public static Data GetEnemy(string soundType)
        {
            return enemiesSet[soundType];
        }

        public void Init()
        {
            enemiesSet.Clear();
        
            for (int i = 0; i < enemies.Count; i++)
            {
                var pair = enemies[i];
                enemiesSet.Add(pair.soundType, pair);
            }
        }
    }
}