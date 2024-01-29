using System;
using LSCore.BattleModule;
using UnityEngine;
using UnityEngine.Scripting;

namespace Battle.Data.Components
{
    [Serializable, Preserve]
    internal class FindHeroComp : FindTargetComp
    {
        public override bool Find(out Transform target)
        {
            target = PlayerWorld.HeroTransform;
            return true;
        }
    }
}