using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using Object = UnityEngine.Object;

namespace GameCore.Battle.Data
{
    public abstract class ObjectsById<T> : SerializedScriptableObject where T : Object
    {
        protected abstract IdGroup IdGroup { get; }
        
        [Serializable]
        private class ObjectByName
        {
            [HideInInspector] public Id id;
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

            public override int GetHashCode() => id.GetInstanceID();
        }
        
        public static Dictionary<string, T> ByName { get; } = new();

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

                    foreach (var id in IdGroup.Ids)
                    {
                        list.Add(id, new ObjectByName(){id = id});
                    }
                }
                
                return list;
            }
        }
#endif
    }
}