using LSCore;
using LSCore.LevelSystem;
using UnityEngine;

namespace Launcher.HeroesManagement
{
    public class HeroStatsPanel : MonoBehaviour
    {
        public HeroId heroId;
        public LevelsManager heroesLevelsManager;
        public FundsByLevelConfig fundsByLevel;
        public LSButton upgradeButton;
        [SerializeReference] public PriceViewSetuper priceSetuper;
        public LSText levelText;
        
        private void OnEnable()
        {
            heroesLevelsManager.SubAndCallLevelChanged(heroId.id, OnLevelChanged);
        }

        private void OnDisable()
        {
            heroesLevelsManager.UnSubLevelChanged(heroId.id, OnLevelChanged);
        }

        private void OnLevelChanged(int level)
        {
            UpgradeHeroLevel.id = heroId;
            var isMaxLevel = heroesLevelsManager.IsMaxLevel(heroId.id);
            
            levelText.text = $"{level}";
            upgradeButton.gameObject.SetActive(!isMaxLevel);
            var data = fundsByLevel.fundsByLevel.GetData(level);
            priceSetuper.Setup(data);
        }
    }
}