using System;
using LSCore.Extensions;
using UnityEngine;

namespace Battle.Data
{
    public class RaidByHeroRank : ScriptableObject
    {
        [Serializable]
        public struct Data
        { 
            public int maxRank;
            public RaidConfig raidConfig;
        }

        [SerializeField] private Data[] data;
        public RaidConfig Current { get; private set; }
        
        public void Init()
        {
            var index = IListExtensions.ClosestBinarySearch(
                index => data[index].maxRank,
                data.Length,
                PlayerData.Config.Level);
            
            Current = data[index].raidConfig;
            Current.Init();
        }
    }
}