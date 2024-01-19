using System;
using UnityEngine;
using UnityEngine.Scripting;

namespace Battle.Data.Components
{
    [Serializable, Preserve]
    internal class FindHeroComponent : FindTargetComponent
    {
        public override bool Find(out Transform target)
        {
            target = PlayerWorld.HeroTransform;
            return true;
        }
    }
}