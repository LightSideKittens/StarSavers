using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Battle.Data.GameProperty
{
    [Serializable]
    public class GamePropertiesByScope
    {
        [field: SerializeField, VerticalGroup(nameof(Scopes)), ValueDropdown(nameof(Scopes))]
        [field: InfoBox("Cannot use multiple identical scopes", InfoMessageType.Error, nameof(isError))]
        public string Scope { get; private set; } = GameScopes.Global;
        [field: OdinSerialize] public List<BaseGameProperty> Properties = new List<BaseGameProperty>();
        
        private static IEnumerable<string> Scopes => GameScopes.Scopes;
        [HideInInspector] public bool isError;
    }
}