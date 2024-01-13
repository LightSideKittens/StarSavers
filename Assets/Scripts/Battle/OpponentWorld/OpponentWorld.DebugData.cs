#if DEBUG
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Battle.Data;
using LSCore;
using UnityEngine.Scripting;

namespace Battle
{
    public partial class OpponentWorld
    {
        static partial void SubscribeOnChange(OnOffPool<Unit> pool)
        {
            pool.Got += OnChange;
            pool.Released += OnChange;
            pool.Destroyed += OnChange;
        }
        
        static partial void AddDebugData()
        {
            SRDebug.Instance.AddOptionContainer(DebugData.data);
        }
        
        static partial void RemoveDebugData()
        {
            SRDebug.Instance.RemoveOptionContainer(DebugData.data);
        }
        
        private class DebugData : INotifyPropertyChanged
        {
            public static readonly DebugData data = new();

            public static void OnChange() => data.OnPropertyChanged(nameof(EnemyCount));

            [Category("Battle Debug Data")]
            [Preserve]
            public int EnemyCount => UnitCount;

            public event PropertyChangedEventHandler PropertyChanged;

            protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private static void OnChange(Unit _) => DebugData.OnChange();
    }
}
#endif