using System;
using LSCore;
using UnityEngine;

namespace BeatHeroes.Windows
{
    public class TestCurrencyShopItemView : MonoBehaviour, IReward
    {
        [SerializeField] public FundView reward;
        [SerializeField] private FundView[] funds;

        private LSButton button;

        private void Awake()
        {
            button = GetComponent<LSButton>();
            button.Listen(Claim);
        }

        private void Claim()
        {
            if (Claim(out var claim))
            {
                claim();
            }
        }
        
        public bool Claim(out Action claim)
        {
            if (funds.Spend(out var spend))
            {
                claim = reward.Earn;
                spend();
                return true;
            }

            claim = null;
            return false;
        }
    }
}