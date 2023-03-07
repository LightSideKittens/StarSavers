using Battle.Windows;
using Core.SingleService;
using GameCore.Battle.Data;
using UnityEngine;

namespace Battle
{
    public class BattleBootstrap : ServiceManager
    {
        [SerializeField] private Units units; 
        [SerializeField] private Cards cards; 
        private void Start()
        {
            units.Init();
            cards.Init();
            DeckWindow.Show();
        }
    }
}