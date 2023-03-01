using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Battle.Data.GameProperty
{
    [Serializable]
    public class GamePropertiesByScope
    {
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
        private int previousCount;
        private bool NeedShowEntityName => !string.IsNullOrEmpty(entityName);

        public IEnumerable<Type> GetFilteredTypeList()
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

        [OnInspectorInit]
        private void OnInspectorInit()
        {
            propsHashCodes = new HashSet<int>();
            for (int i = 0; i < Properties.Count; i++)
            {
                var prop = Properties[i];
                prop.scope = Scope;
                var type = prop.GetType();
                var wasRemoved = false;
                
                if (BaseGameProperty.IconsByType.ContainsKey(type) && level != 1)
                {
                    if (!notEntityPropertyTypes.Contains(type))
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

        private void GetRecentlyChangedProperty()
        {
            var wasReanalyzed = false;
            if (propsHashCodes != null)
            {
                for (int i = 0; i < Properties.Count; i++)
                {
                    var prop = Properties[i];
                    
                    if (!propsHashCodes.Contains(prop.GetHashCode()))
                    {
                        wasReanalyzed = true;
                        break;
                    }
                }
            }
            else
            {
                propsHashCodes = new HashSet<int>();
                for (int i = 0; i < Properties.Count; i++)
                {
                    propsHashCodes.Add(Properties[i].GetHashCode());
                }
            }

            if (wasReanalyzed)
            {
                if (Properties.Count == previousCount)
                {
                    LevelConfig.ReanalyzeScope(this);
                }

                propsHashCodes.Clear();
                for (int i = 0; i < Properties.Count; i++)
                {
                    propsHashCodes.Add(Properties[i].GetHashCode());
                    ResolveDependencies(Properties[i]);
                }
            }

            previousCount = Properties.Count;
        }
        
        public void OnGUI()
        {
            GetRecentlyChangedProperty();
        }

        private void ResolveDependencies(BaseGameProperty property)
        {
            property.scope = Scope;
            if (property.GetType() == typeof(RicochetGP) && !addedTypes.Contains(typeof(DamageGP)))
            {
                Properties.Add(new DamageGP());
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
            InitAddedTypes();
            for (int i = 0; i < Properties.Count; i++)
            {
                ResolveDependencies(Properties[i]);
            }
            
            ScopeChanged?.Invoke();
        }
#endif
    }
}