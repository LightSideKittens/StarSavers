using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GameCore.Battle.Data
{
    public class ObjectByKey<Key, Value> : SerializedScriptableObject where Value : Object
    {
        [Serializable]
        public struct Pair
        {
            public Key key;
            public Value value;
        }
        
        public static Dictionary<Key, Value> ByKey { get; } = new(); 
        [SerializeField] private List<Pair> objectsByKey = new ();

        public void Init()
        {
            for (int i = 0; i < objectsByKey.Count; i++)
            {
                var data = objectsByKey[i];
                ByKey.TryAdd(data.key, data.value);
            }
        }
    }
}