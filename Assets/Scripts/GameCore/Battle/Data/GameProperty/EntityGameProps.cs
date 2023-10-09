using System;
using System.Collections.Generic;
using GameCore.Battle.Data;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace Battle.Data.GameProperty
{
    [Serializable]
    public class EntityGameProps
    {
        [ValueDropdown(nameof(Destinations))]
        public string Destination { get; private set; } 
        [field: OdinSerialize] public HashSet<BaseGameProperty> Props { get; } = new();

        public virtual IList<ValueDropdownItem<int>> Destinations => IdToName.GetValues(EntityMeta.EntityNames);
    }

    [Serializable]
    public class AllDestinationsGameProps : EntityGameProps
    {
        public override IList<ValueDropdownItem<int>> Destinations => IdToName.GetValues(EntityMeta.AllDestinations);
    }
}