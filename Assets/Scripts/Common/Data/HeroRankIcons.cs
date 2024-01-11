using Common.Data;
using UnityEngine;

namespace BeatHeroes.Data
{
    public class HeroRankIcons : ScriptableObject
    {
        [field: SerializeField] public DataByHeroRank<Sprite> Icons { get; private set; }
    }
}