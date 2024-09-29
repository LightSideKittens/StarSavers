using LSCore;
using LSCore.BattleModule;
using LSCore.LevelSystem;
using UnityEngine;

namespace Launcher.HeroesManagement
{
    public class HeroStatsPanel : MonoBehaviour
    {
        public HeroId heroId;
        public LevelsManager heroesLevelsManager;
        public LSButton upgradeButton;
        public LSButton[] upgradeButtons;
        public LSText levelText;

        private void Awake()
        {
            heroesLevelsManager.GetCurrentLevel<Unit>(heroId.id);
        }
    }
}