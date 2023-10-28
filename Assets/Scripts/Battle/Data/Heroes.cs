using System.Collections.Generic;
using Battle.Data;

namespace GameCore.Battle.Data
{
    public class Heroes : ObjectsByEntityId<Unit>
    {
        protected override HashSet<int> Scope { get; }
    }
}