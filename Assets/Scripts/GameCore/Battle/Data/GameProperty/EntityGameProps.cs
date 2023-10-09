using System;
using System.Collections.Generic;
using GameCore.Battle.Data;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Battle.Data.GameProperty
{
    [Serializable]
    public class EntityGameProps
    {
        [field: ValueDropdown(nameof(Destinations))] 
        [field: SerializeField] public int Destination { get; private set; } 
        
        [field: HideReferenceObjectPicker]
        [field: ValueDropdown(nameof(AvailableProps), IsUniqueList = true)] 
        [field: OdinSerialize] public HashSet<BaseGameProperty> Props { get; } = new();

        public virtual IList<ValueDropdownItem<int>> Destinations => IdToName.GetValues(EntityMeta.EntityIds);

        public virtual IList<ValueDropdownItem<BaseGameProperty>> AvailableProps
        {
            get
            {
                var list = new ValueDropdownList<BaseGameProperty>();
                list.Add(nameof(HealthGP), new HealthGP());
                list.Add(nameof(AttackSpeedGP), new AttackSpeedGP());
                return list;
            }
        }
    }

    [Serializable]
    public class AllDestinationsGameProps : EntityGameProps
    {
        public override IList<ValueDropdownItem<int>> Destinations => IdToName.GetValues(EntityMeta.AllDestinations, new HashSet<int>(){2});
    }
}