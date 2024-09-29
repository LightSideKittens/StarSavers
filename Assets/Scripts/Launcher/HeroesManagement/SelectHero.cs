using System;
using Battle.Data;

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
}