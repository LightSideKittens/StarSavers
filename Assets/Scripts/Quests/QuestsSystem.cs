using System;
using LSCore.LifecycleSystem;

namespace StarSavers
{
    public class QuestsSystem : LifecycleSystem<QuestsSystem>
    {
        [Serializable]
        public class CreateQuests : CreateLifecycleObjects { }
    }
}