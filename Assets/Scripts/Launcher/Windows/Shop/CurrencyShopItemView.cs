using System;
using LSCore;
using UnityEngine;
using Action = System.Action;

namespace StarSavers.Windows
{
    public class CurrencyShopItemView : MonoBehaviour
    {
        [SerializeField] private Funds reward;
        [SerializeField] private Funds cost;

        private LSButton button;

        private void Awake()
        {
            button = GetComponent<LSButton>();
            button.Clicked += Claim;
        }

        private void Claim()
        {
            if (Claim(out var claim))
            {
                claim();
            }
        }

        private bool Claim(out Action claim)
        {
            if (cost.Spend(out var spend))
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