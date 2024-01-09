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
            PlayerData.TryGetSelectedHeroRank(out var rank);
            var index = IListExtensions.ClosestBinarySearch(
                index => data[index].maxRank,
                data.Length,
                rank);
            
            Current = data[index].raidConfig;
            Current.Init();
        }
    }
}