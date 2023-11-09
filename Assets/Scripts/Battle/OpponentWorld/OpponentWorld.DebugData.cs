#if DEBUG
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Battle.Data;
using UnityEngine;
using UnityEngine.Scripting;

namespace Battle
{
    public partial class OpponentWorld
    {
        public class DebugData : INotifyPropertyChanged
        {
            private static readonly DebugData data = new();

            [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
            public static void OnStartup()
            {
                SRDebug.Instance.AddOptionContainer(data);
            }

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