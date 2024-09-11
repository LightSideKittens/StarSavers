using LSCore;
using LSCore.Extensions;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Battle.Data
{
    public class RaidSetupper : ScriptableObject
    {
        [SerializeField] private Intervals heroRankIntervals;
        [SerializeField] private RaidConfigRef[] raids;
        
        public RaidConfig Current { get; private set; }
        
        public AsyncOperationHandle<RaidConfig> Setup()
        {
            PlayerData.TryGetSelectedHeroRank(out var rank);
            return raids.Random().LoadAsync().OnComplete(raid =>
            {
                Current = raid;
                Current.Setup(heroRankIntervals.Get(rank).index + 1);
            });
        }
    }
}