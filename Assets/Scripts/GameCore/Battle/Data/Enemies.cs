using System.Collections.Generic;
using Battle.Data;

namespace GameCore.Battle.Data
{
    public class Enemies : ObjectsByEntityId<Unit>
    {
        protected override HashSet<int> Scope => EntityMeta.GetGroupByName("Enemies").AllEntityIds;
    }
}