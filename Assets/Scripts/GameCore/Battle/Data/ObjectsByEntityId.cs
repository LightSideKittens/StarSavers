using System;
using System.Collections.Generic;
using Battle.Data;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using Object = UnityEngine.Object;

namespace GameCore.Battle.Data
{
    public abstract class ObjectsByEntityId<T> : SerializedScriptableObject where T : Object
    {
        protected virtual HashSet<int> Scope => EntityMeta.EntityIds.Ids;
        
        [Serializable]
        private class ObjectByName
        {
            [HideInInspector] public int id;
            public T obj;
            
            public override bool Equals(object obj)
            {
                if (obj is ObjectByName drawer)
                {
                    return Equals(drawer);
                }

                return false;
            }
        
            public bool Equals(ObjectByName other) => id == other.id;

            public override int GetHashCode() => id;
        }
        
        public static Dictionary<int, T> ByName { get; } = new();

        [HideReferenceObjectPicker]
        [ValueDropdown("AvailableObjects", IsUniqueList = true)]
        [OdinSerialize]
        private HashSet<ObjectByName> byName;

        public void Init()
        {   
            ByName.Clear();

            foreach (var data in byName)
            {
                ByName.TryAdd(data.id, data.obj);
            }
        }

#if UNITY_EDITOR
        private ValueDropdownList<ObjectByName> list;
        private IList<ValueDropdownItem<ObjectByName>> AvailableObjects
        {
            get
            {
                if (list == null)
                { 
                    list = new ValueDropdownList<ObjectByName>();

                    foreach (var id in Scope)
                    {
                        list.Add(EntityMeta.EntityIds.GetNameById(id), new ObjectByName(){id = id});
                    }
                }
                
                return list;
            }
        }
#endif
    }
}