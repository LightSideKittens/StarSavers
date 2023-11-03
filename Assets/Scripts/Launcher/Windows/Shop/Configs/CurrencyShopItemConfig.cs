using System.Collections.Generic;
using LSCore;
using UnityEngine;

namespace BeatHeroes.Windows
{
    
    [CreateAssetMenu(fileName = nameof(CurrencyShopItemConfig), menuName = "Launcher/" + nameof(CurrencyShopItemConfig), order = 0)]
    public class CurrencyShopItemConfig : BaseShopItemConfig<CurrencyShopItemConfig>
    {
        public override IEnumerable<IReward> rewards
        {
            get
            { 
                yield return currencyRewardConfig;
            }
        }

        [SerializeField] private CurrencyRewardConfig currencyRewardConfig;
        protected override CurrencyShopItemConfig This => this;
    }
}