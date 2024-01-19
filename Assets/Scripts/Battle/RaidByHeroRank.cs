using LSCore;
using UnityEngine;

namespace Battle.Data
{
    public class RaidByHeroRank : ScriptableObject
    {
        [SerializeField] private DataByInterval<RaidConfig> raids;
        
        public RaidConfig Current { get; private set; }
        
        public void Setup()
        {
            PlayerData.TryGetSelectedHeroRank(out var rank);
            Current = raids.Get(rank).data;
            Current.Setup();
        }
    }
}