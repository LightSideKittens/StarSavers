using System;
using UnityEngine;

namespace Battle.Data
{
    public class Locations : ScriptableObject
    {
        [Serializable]
        public struct Data
        {
            public int maxLevel;
            public LocationRef locationRef;
        }
        
        [SerializeField] private Data[] locations;
        public Data this[int index] => locations[index];
        public int Length => locations.Length;
        
        public void GetLocationByLevel(int level)
        {
            
        }
    }
}