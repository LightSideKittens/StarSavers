using System;
using Battle.Data;
using LSCore;

namespace Common.Data
{
    [Serializable]
    public class DataByHeroRank<T> : DataByInterval<T>
    {
        public bool TryGetSelectedHeroData(out (T data, int rank, int to) data) => TryGet(PlayerData.Config.SelectedHero, out data);
        
        public bool TryGet(string heroId, out (T data, int rank, int to) value)
        {
            value = default;
            var result = PlayerData.TryGetRank(heroId, out value.rank);
            (value.data, value.to) = Get(value.rank);
            return result;
        }
        
#if UNITY_EDITOR
        protected override string Label => "Hero Rank";
#endif
    }
}