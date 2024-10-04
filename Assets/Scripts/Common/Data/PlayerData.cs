using System.Collections.Generic;
using LSCore.ConfigModule;

namespace Battle.Data
{
    public class PlayerData : GameSingleConfig<PlayerData>
    {
        public ReactProp<string> SelectedHero { get; set; } = (ReactProp<string>)"";
        
        public int Rank { get; set; }
        public Dictionary<string, int> RankByHero { get; } = new();

        public static bool TryGetRank(string heroId, out int rank) => Config.RankByHero.TryGetValue(heroId, out rank);
        public static bool TryGetSelectedHeroRank(out int rank) => Config.RankByHero.TryGetValue(Config.SelectedHero.Value, out rank);
        public static bool IsSelected(string id) => Config.SelectedHero.Value == id;
    }
}