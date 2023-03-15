using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEditor;
using UnityEngine;

namespace Battle.Data.GameProperty
{
    [Serializable]
    public class GamePropertiesByScope
    {
        [field: OnValueChanged("OnScopeChanged")]
        [field: SerializeField, VerticalGroup("Scopes"), ValueDropdown("Scopes")]
        [field: InfoBox("Cannot use multiple identical scopes", InfoMessageType.Error, "isMultipleIdenticalScopeError")]
        [field: InfoBox("Scope properties is empty!", InfoMessageType.Error, "isEmptyScopeError")]
        public string Scope { get; private set; } = "Global";
        
        [field: TypeFilter("FilteredTypeList")]
        [field: OnValueChanged("OnScopeChanged")]
        [field: OdinSerialize] public List<BaseGameProperty> Properties { get; } = new();

#if UNITY_EDITOR
        private static IEnumerable<string> Scopes => GameScopes.Scopes;
        private static HashSet<Type> notEntityPropertyTypes = new()
        {
            typeof(HealthGP),
            typeof(DamageGP),
        };
        
        private static HashSet<Type> updatablePropertyTypes = new()
        {
            typeof(RicochetGP),
        };
        
        private static HashSet<Type> effectorPropertyTypes = new()
        {
            typeof(HealthGP),
            typeof(DamageGP),
            typeof(RadiusGP),
            typeof(MoveSpeedGP),
            typeof(AttackSpeedGP),
        };
        
        public event Action ScopeChanged;
        private HashSet<Type> addedTypes = new ();

        [ShowIf("$" + nameof(NeedShowEntityName)), VerticalGroup(nameof(Scopes))]
        [ReadOnly] public string entityName;

        [HideInInspector] public int level;
        [HideInInspector] public bool isMultipleIdenticalScopeError;
        [HideInInspector] public bool isEmptyScopeError;
        private bool NeedShowEntityName => !string.IsNullOrEmpty(entityName);

        public IEnumerable<Type> FilteredTypeList
        {
            get
            {
                InitAddedTypes();
                var isEntity = GameScopes.IsEntityScope(Scope);
                var notAddedTypes = Enumerable.Empty<Type>();

                if (Scope.Contains("Effectors"))
                {
                    notAddedTypes = notAddedTypes.Concat(GetNotAddedTypes(effectorPropertyTypes));
                }
                else if (isEntity && level == 1)
                {
                    notAddedTypes = notAddedTypes.Concat(GetNotAddedTypes(BaseGameProperty.IconsByType.Keys));
                }
                else
                {
                    notAddedTypes = notAddedTypes.Concat(GetNotAddedTypes(notEntityPropertyTypes));

                    if (isEntity)
                    {
                        notAddedTypes = notAddedTypes.Concat(GetNotAddedTypes(updatablePropertyTypes));
                    }
                }

                return notAddedTypes;

                IEnumerable<Type> GetNotAddedTypes(IEnumerable<Type> types)
                {
                    foreach(var type in types)
                    {
                        if (!addedTypes.Contains(type))
                        {
                            yield return type;
                        }
                    }
                }
            }
        }

        [OnInspectorInit]
        private void OnInspectorInit()
        {
            for (int i = 0; i < Properties.Count; i++)
            {
                var prop = Properties[i];
                prop.scope = Scope;
                var type = prop.GetType();

                if (BaseGameProperty.IconsByType.ContainsKey(type) && level != 1)
                {
                    if (!notEntityPropertyTypes.Contains(type))
                    {
                        Properties.Remove(prop);
                        i--;
                    }
                }
            }
        }
        
        public void OnGUI()
        {
            for (int i = 0; i < Properties.Count; i++)
            {
                Properties[i].scope = Scope;
            }
        }
        
        [OnInspectorDispose]
        private void OnInspectorDispose()
        {
            
        }
        
        private void ResolveAllDependencies()
        {
            InitAddedTypes();
            for (int i = 0; i < Properties.Count; i++)
            {
                var prop = Properties[i];
                prop.scope = Scope;
                if (prop.GetType() == typeof(RicochetGP) && !addedTypes.Contains(typeof(DamageGP)))
                {
                    Properties.Add(new DamageGP());
                }
            }
        }

        private void InitAddedTypes()
        {
            addedTypes = new HashSet<Type>();
            for (int i = 0; i < Properties.Count; i++)
            {
                addedTypes.Add( Properties[i].GetType());
            }
        }

        private void OnScopeChanged()
        {
            ResolveAllDependencies();
            ScopeChanged?.Invoke();
        }
#endif
    }
}