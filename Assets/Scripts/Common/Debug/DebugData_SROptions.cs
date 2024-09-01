#if DEBUG
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Scripting;

namespace StarSavers
{
    public partial class DebugData
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        public static void OnStartup()
        {
            SRDebug.Instance.AddOptionContainer(Config);
        }
        
        [Category("Debug Data")]
        [Preserve]
        public bool NeedShowRadius
        {
            get => needShowRadius;
            set => needShowRadius = value;
        }
    
        [Category("Debug Data")]
        [Preserve]
        public bool ServerEnabled
        {
            get => serverEnabled;
            set => serverEnabled = value;
        }
    }
}
#endif