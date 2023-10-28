using System.Collections.Generic;
using Battle.Data;

namespace GameCore.Battle.Data
{
    public class Heroes : ObjectsById<Unit>
    {
        protected override IdGroup IdGroup { get; }
    }
}