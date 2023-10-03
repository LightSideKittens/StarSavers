using System.Collections.Generic;
using Battle.Data;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GameCore.Battle.Data
{
    public class LocationData : ScriptableObject
    {
        public Location prefab;
        protected virtual IEnumerable<string> Entities => GameScopes.EntitiesNames;
        
        [ValueDropdown(nameof(Entities))] public string[] entityNames;
    }
}