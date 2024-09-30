using System;
using Battle.Data;
using LSCore.LevelSystem;

namespace Launcher.HeroesManagement
{
    [Serializable]
    public class SelectHero : LSAction
    {
        public HeroId id;
        
        public override void Invoke()
        {
            PlayerData.Config.SelectedHero.Value = id.id;
        }
    }
    
    [Serializable]
    public class UpgradeHeroLevel : LSAction
    {
        public static HeroId id;
        public LevelsManager heroesLevelsManager;
        
        public override void Invoke()
        {
            heroesLevelsManager.UpgradeLevel(id.id);
        }
    }
}