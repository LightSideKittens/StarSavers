using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.Serialization;

namespace Battle.Data.GameProperty
{
    [Serializable]
    public class GamePropertiesByScope
    {
        private static IEnumerable<string> Scopes => GameScopes.Scopes;
        private static HashSet<Type> notEntityPropertyType = new()
        {
            typeof(HealthGP),
            typeof(DamageGP),
        };

        [field: OnValueChanged(nameof(OnScopeChanged))]
        [field: SerializeField, VerticalGroup(nameof(Scopes)), ValueDropdown(nameof(Scopes))]
        [field: InfoBox("Cannot use multiple identical scopes", InfoMessageType.Error, nameof(isMultipleIdenticalScopeError))]
        [field: InfoBox("Scope properties is empty!", InfoMessageType.Error, nameof(isEmptyScopeError))]
        public string Scope { get; private set; } = "Global";
        
        [field: TypeFilter("GetFilteredTypeList")]
        [field: OnValueChanged(nameof(OnScopeChanged))]
        [field: OdinSerialize] public List<BaseGameProperty> Properties = new List<BaseGameProperty>();

#if UNITY_EDITOR
        public event Action ScopeChanged;
        private HashSet<int> propsHashCodes;
        private HashSet<Type> addedTypes = new ();

        [ShowIf("$" + nameof(NeedShowEntityName)), VerticalGroup(nameof(Scopes))]
        [ReadOnly] public string entityName;

        [HideInInspector] public int level;
        [HideInInspector] public bool isMultipleIdenticalScopeError;
        [HideInInspector] public bool isEmptyScopeError;

        private bool NeedShowEntityName => !string.IsNullOrEmpty(entityName);

        public IEnumerable<Type> GetFilteredTypeList()
        {
            InitAddedTypes();
            var isEntity = GameScopes.IsEntityScope(Scope);

            if (isEntity && level == 1)
            {
                foreach (var type in BaseGameProperty.IconsByType.Keys)
                {
                    if (!addedTypes.Contains(type))
                    {
                        yield return type;
                    }
                }
            }
            else
            {
                foreach(var type in notEntityPropertyType)
                {
                    if (!addedTypes.Contains(type))
                    {
                        yield return type;
                    }
                }
            }
        }

        [OnInspectorInit]
        private void OnInspectorInit()
        {
            propsHashCodes = new HashSet<int>();
            for (int i = 0; i < Properties.Count; i++)
            {
                var prop = Properties[i];
                var type = prop.GetType();
                var wasRemoved = false;
                
                if (BaseGameProperty.IconsByType.ContainsKey(type) && level != 1)
                {
                    if (!notEntityPropertyType.Contains(type))
                    {
                        Properties.Remove(prop);
                        i--;
                        wasRemoved = true;
                    }
                }

                if (!wasRemoved)
                {
                    propsHashCodes.Add(Properties[i].GetHashCode());
                }
            }
        }
        
        public void OnGUI()
        {
            if (propsHashCodes != null)
            {
                for (int i = 0; i < Properties.Count; i++)
                {
                    if (!propsHashCodes.Contains(Properties[i].GetHashCode()))
                    {
                        LevelConfig.ReanalyzeScope(this);
                    }
                }
            }
        }

        private void InitAddedTypes()
        {
            addedTypes = new HashSet<Type>();
            for (int i = 0; i < Properties.Count; i++)
            {
                addedTypes.Add(Properties[i].GetType());
            }
        }
        
        private void OnScopeChanged()
        {
            ScopeChanged?.Invoke();
        }
#endif
    }
}