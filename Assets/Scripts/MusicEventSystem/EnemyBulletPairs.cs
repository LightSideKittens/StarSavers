using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using UnityEngine;

namespace DefaultNamespace.MusicEventSystem
{
    [CreateAssetMenu(fileName = "EnemyBulletPairs", menuName = "MusicEventSystem/EnemyBulletPairs", order = 0)]
    public class EnemyBulletPairs : ScriptableObject
    {
        [Serializable]
        public struct Pair
        {
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
}