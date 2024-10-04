using LSCore;
using LSCore.LevelSystem;
using UnityEngine;

namespace Launcher.HeroesManagement
{
    public class HeroOpenButton : MonoBehaviour
    {
        public HeroId heroId;
        public LevelsManager heroesLevelsManager;
        public FundsByLevelConfig fundsByLevel;
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
            levelText.text = $"{level}";
            var data = fundsByLevel.fundsByLevel.GetData(level);
            priceSetuper.Setup(data);
        }
    }
}
