using System.Collections.Generic;
using Battle.Data;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GameCore.Battle.Data
{
    public class Location : ScriptableObject
    {
        public GameObject prefab;

        [ValueDropdown("Enemies", IsUniqueList = true)] public int[] enemies;

#if UNITY_EDITOR
        protected IList<ValueDropdownItem<int>> Enemies => EntityMeta.GetGroupByName("Enemies").EntityValues;
#endif
    }
}