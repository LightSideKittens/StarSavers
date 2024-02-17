using System;
using LSCore.ConditionModule;
using static LSCore.BattleModule.FindTargetComp;

namespace Battle.Data.Components.TargetProviders
{
    [Serializable]
    internal class IsEnemy : Condition
    {
        protected override bool Check() => targetUnit.TeamId != selfUnit.TeamId;
    }
}