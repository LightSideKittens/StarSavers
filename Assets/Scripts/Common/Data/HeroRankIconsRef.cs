using System;
using LSCore.AddressablesModule.AssetReferences;

namespace BeatHeroes.Data
{
    [Serializable]
    public class HeroRankIconsRef : AssetRef<HeroRankIcons>
    {
        public HeroRankIconsRef(string guid) : base(guid) { }
    }
}
