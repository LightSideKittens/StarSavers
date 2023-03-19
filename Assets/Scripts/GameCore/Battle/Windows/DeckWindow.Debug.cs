#if DEBUG
using Core.ConfigModule;

namespace Battle.Windows
{
    public partial class DeckWindow
    {
        public static bool InfinityMana
        {
            get => DebugData.Config.infinityMana;
            set
            {
                DebugData.Config.infinityMana = value;

                if (value)
                {
                    Instance.mana = 10;
                    Instance.UpdateManaView();
                    Instance.mana = 10000;
                }
                
                Instance.manaEnabled = !value;
            }
        }
    }
}
#endif