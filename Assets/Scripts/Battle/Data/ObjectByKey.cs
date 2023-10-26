using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GameCore.Battle.Data
{
    public class ObjectByKey<TKey, TValue> : SerializedScriptableObject where TValue : Object
    {
        [Serializable]
        public struct Pair
        {
            public TKey key;
            public TValue value;
        }
        
        public static Dictionary<TKey, TValue> ByKey { get; } = new(); 
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