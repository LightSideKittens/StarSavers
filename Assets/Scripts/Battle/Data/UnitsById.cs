using LSCore;
using LSCore.LevelSystem;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Battle.Data
{
    public class UnitsById : ValuesById<Unit>
    {
        [SerializeField] private LevelIdGroup group;

        protected override void SetupDataSelector(ValueDropdownList<Data> list) => SetupByGroup(group, list);
    }
}