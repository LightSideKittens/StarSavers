#if DEBUG

namespace Battle.Windows
{
    public partial class DeckWindow
    {
        private bool infinityMana;
        
        public static bool InfinityMana
        {
            get => Instance.infinityMana;
            set
            {
                Instance.infinityMana = value;

                if (value)
                {
                    Instance.mana = int.MaxValue;
                    Instance.UpdateManaView();
                }
                
                Instance.manaEnabled = !value;
            }
        }
    }
}
#endif