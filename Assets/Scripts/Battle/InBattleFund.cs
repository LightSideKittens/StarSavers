using System;
using System.Collections.Generic;
using LSCore;

namespace Battle.Data
{
    [Serializable]
    public class InBattleFund : Fund
    {
#if UNITY_EDITOR
        protected override IEnumerable<CurrencyIdGroup> Groups
        {
            get
            {
                yield return AssetDatabaseUtils.LoadAny<CurrencyIdGroup>("InBattle");
            }
        }
#endif
    }
}