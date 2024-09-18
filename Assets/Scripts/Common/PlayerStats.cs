using LSCore.ConfigModule;

namespace StarSavers
{
    public class PlayerStats : GameSingleConfig<PlayerStats>
    {
        public new static PlayerStats Config => GameSingleConfig<PlayerStats>.Config;
        
        public int defeatedEnemies;
    }
}