using System.Linq;
using LSCore;
using LSCore.AddressablesModule.AssetReferences;
using UnityEngine;
using UnityEngine.UI;

namespace BeatHeroes.Windows
{
    public class CurrencyShopItemView : BaseShopItemView<CurrencyShopItemConfig>
    {
        [SerializeField] private LSButton button;
        [SerializeField] private LSText title;
        [SerializeField] private Image preview;
        [SerializeField] private LSText count;
        
        private CurrencyShopItemConfig config;
        
        public override void Setup(CurrencyShopItemConfig config)
        {
            this.config = config;
            
            if (this.config)
            {
                SetupView();
            }
        }

        private void SetupView()
        {
            var reward = config.rewards.First() as CurrencyRewardConfig;
            
            if (!reward)
            {
                return;
            }
            
            title.text =  reward.title;
            preview.sprite = reward.preview.Load();
            count.text = $"{reward.Fund.value}";
            button.Listen(OnClaim);
        }

        private void Clear()
        {
            button.UnListen(OnClaim);
        }

        private void OnClaim()
        {
            if (config.Claim(out var claim))
            {
                claim();
            }
        }
    }
}