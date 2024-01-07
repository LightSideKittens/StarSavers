using System;
using System.Collections.Generic;
using LSCore;

namespace Battle.Data
{
    [Serializable]
    public class InBattleFund : Fund
    {
        public override void Earn()
        {
            BattleWorld.Funds.TryGetValue(id, out var value);
            BattleWorld.Funds[id] = value + Value;
        }

        public override bool Spend(out Action spend)
        {
            BattleWorld.Funds.TryGetValue(id, out var value);
            
            if (value >= Value)
            {
                spend = () => BattleWorld.Funds[id] = value - Value;
                return true;
            }

            spend = null;
            return false;
        }

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