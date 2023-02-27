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
        private static IEnumerable<string> Scopes => GameScopes.Scopes;
        
        [field: SerializeField, VerticalGroup(nameof(Scopes)), ValueDropdown(nameof(Scopes))]
        [field: InfoBox("Cannot use multiple identical scopes", InfoMessageType.Error, nameof(isError))]
        public string Scope { get; private set; } = "Global";
        [field: OdinSerialize] public List<BaseGameProperty> Properties = new List<BaseGameProperty>();

        [ShowIf("$" + nameof(NeedShowEntityName)), VerticalGroup(nameof(Scopes))]
        [ReadOnly] public string entityName;

        [HideInInspector] public bool isError;

        private bool NeedShowEntityName => !string.IsNullOrEmpty(entityName);
    }
}