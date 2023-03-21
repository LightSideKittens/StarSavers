#if DEBUG
using static Core.ConfigModule.BaseConfig<Core.ConfigModule.DebugData>;

namespace Battle.Windows
{
    public partial class DeckWindow
    {
        partial void OnInit()
        {
            SetInfinityMana(Config.infinityMana);
        }

        private static void SetInfinityMana(bool isInfinity)
        {
            if (isInfinity)
            {
                Instance.mana = 10;
                Instance.UpdateManaView();
                Instance.mana = 10000;
            }
                
            Instance.manaEnabled = !isInfinity;
        }

        public static bool InfinityMana
        {
            get => Config.infinityMana;
            set
            {
                Config.infinityMana = value;

                if (!IsNull)
                {
                    SetInfinityMana(Config.infinityMana);
                }
            }
        }
    }
}
#endif