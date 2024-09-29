using LSCore;
using LSCore.LevelSystem;
using Sirenix.OdinInspector;
using UnityEngine;

namespace StarSavers
{
    public class HeroesRenderersById : ValueById<GameObject>
    {
        public LevelIdGroup allHeroes;
        
#if UNITY_EDITOR
        protected override void SetupDataSelector(ValueDropdownList<Entry> list) => SetupByGroup(allHeroes, list);
#endif
    }
}