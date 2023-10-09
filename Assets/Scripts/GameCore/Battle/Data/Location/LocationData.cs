using System.Collections.Generic;
using Battle.Data;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GameCore.Battle.Data
{
    public class LocationData : ScriptableObject
    {
        public Location prefab;
        protected virtual IList<ValueDropdownItem<int>> Enemies => IdToName.GetValues(EntityMeta.EntityIds);
        
        [ValueDropdown(nameof(Enemies))] public string[] enemies;
    }
}