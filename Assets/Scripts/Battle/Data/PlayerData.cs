using LSCore.ConfigModule;

namespace GameCore.Battle.Data
{
    public class PlayerData : BaseConfig<PlayerData>
    {
        public int SelectedHero { get; set; }
        public int Level { get; set; }

        protected override void SetDefault()
        {
            base.SetDefault();
            SelectedHero = 0;
            Level = 1;
        }
    }
}