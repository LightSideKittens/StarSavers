using LSCore.ConfigModule;

namespace GameCore.Battle.Data
{
    public class PlayerData : BaseConfig<PlayerData>
    {
        public string SelectedHero { get; set; }
        public int Level { get; set; }

        protected override void SetDefault()
        {
            base.SetDefault();
            SelectedHero = "Arcane";
            Level = 1;
        }
    }
}