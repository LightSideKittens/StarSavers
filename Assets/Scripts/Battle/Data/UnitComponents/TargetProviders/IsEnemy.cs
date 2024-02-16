using System;
using LSCore.BattleModule;
using static LSCore.BattleModule.FindTargetComp;

namespace Battle.Data.Components.TargetProviders
{
    [Serializable]
    internal class IsEnemy : TargetChecker
    {
        protected override bool Check() => targetUnit.TeamId != selfUnit.TeamId;
    }
}