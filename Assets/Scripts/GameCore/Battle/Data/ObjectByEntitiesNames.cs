using System;
using System.Collections.Generic;
using Battle.Data;
using Sirenix.OdinInspector;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;

namespace GameCore.Battle.Data
{
    public abstract class ObjectByEntitiesNames<T> : SerializedScriptableObject where T : Object
    {
        private static IList<ValueDropdownItem<int>> AllEntities => IdToName.ValuesFunction(EntityMeta.EntityNames);
        
        [Serializable]
        private class ObjectByName
        {
            [ValueDropdown(nameof(Entities))]
            public int name;
            public T obj;
            
            private IList<ValueDropdownItem<int>> Entities => AllEntities;
        }
        
        public static Dictionary<int, T> ByName { get; } = new(); 
        [SerializeField] private List<ObjectByName> byName = new ();

        public void Init()
        {   
            ByName.Clear();
            
            for (int i = 0; i < byName.Count; i++)
            {
                var data = byName[i];
                ByName.TryAdd(data.name, data.obj);
            }
        }
    }
}