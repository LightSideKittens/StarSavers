using LSCore;
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
            levelText.text = $"{level}";
            var parent = upgradeButton.transform.parent;
            level = Mathf.Clamp(level, 0, upgradeButtons.Length);
            var targetButton = upgradeButtons[level - 1];
            Destroy(upgradeButton.gameObject);
            Instantiate(targetButton, parent);
        }
    }
}