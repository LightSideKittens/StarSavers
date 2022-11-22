using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using System.Diagnostics;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "EnemyBulletPairs", menuName = "MusicEventSystem/EnemyBulletPairs", order = 0)]
public class EnemyBulletPairs : ScriptableObject
{
    [Serializable]
    public struct Pair
    {
        [Serializable]
        public struct Enemy
        {
            public GameObject prefab;
            public float health;
        }
        
        [ValueDropdown("Types")]
        public string soundType;
        public GameObject enemy;
        public GameObject bullet;
        
        [UsedImplicitly]
        private static IEnumerable<string> Types => SoundTypes.Types;
    }

    [SerializeField] private List<Pair> pairs;
    public static Dictionary<string, Pair> Pairs { get; private set; } = new ();

    public void Init()
    {
        for (int i = 0; i < pairs.Count; i++)
        {
            var pair = pairs[i];
            Pairs.Add(pair.soundType, pair);
        }
    }
}