using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GameCore.Battle.Data
{
    [Serializable]
    public class IdToName : List<IdToName.Data>
    {
        [Serializable]
        public class Data
        {
            [NonSerialized] public IdToName set;
            [NonSerialized] public int prevId;
            [NonSerialized] public string prevName;

            [OnValueChanged("IdChanged")] public int id;
            [OnValueChanged("NameChanged")] public string name;

            public void Init(IdToName set)
            {
                this.set = set;
                prevId = id;
                prevName = name;
            }

            private void IdChanged()
            {
                if (set.ids.Contains(id))
                {
                    id = prevId;
                }
                else
                {
                    set.ids.Remove(prevId);
                    set.nameById.Remove(prevId);
                    
                    prevId = id;
                    
                    set.ids.Add(prevId);
                    set.nameById.Add(prevId, name);
                }
            }

            private void NameChanged()
            {
                if (set.names.Contains(name))
                {
                    name = prevName;
                }
                else
                {
                    set.names.Remove(prevName);
                    prevName = name;
                    set.names.Add(prevName);
                    set.nameById[id] = prevName;
                }
            }
        }

        private HashSet<int> ids = new();
        private HashSet<string> names = new();
        private Dictionary<int, string> nameById = new();

        public event Action Changed;

        public IdToName()
        {
            Init();
        }
        
        public void Init()
        {
            ids.Clear();
            names.Clear();
            nameById.Clear();
            
            foreach (var data1 in this)
            {
                data1.Init(this);
                ids.Add(data1.id);
                names.Add(data1.name);
                nameById.Add(data1.id, data1.name);
            }
        }

        public bool CreateData()
        {
            var hash = Random.Range(-999999, 999999);
            var name = $"Entity Name {hash}";

            if (ids.Add(hash) && names.Add(name))
            {
                var data = new Data{ name = $"Entity Name {hash}", id = hash };
                data.Init(this);
                base.Add(data);
                nameById.Add(data.id, data.name);
                Changed?.Invoke();
                return true;
            }

            return false;
        }

        public new bool Add(Data data)
        {
            if (ids.Add(data.id) && names.Add(data.name))
            {
                data.Init(this);
                base.Add(data);
                nameById.Add(data.id, data.name);
                Changed?.Invoke();
                return true;
            }
            
            return false;
        }
        
        public new void Remove(Data data)
        {
            InternalRemove(data);
            base.Remove(data);
            Changed?.Invoke();
        }
        
        public new void RemoveAt(int index)
        {
            InternalRemove(this[index]);
            base.RemoveAt(index);
            Changed?.Invoke();
        }

        private void InternalRemove(Data data)
        {
            ids.Remove(data.id);
            names.Remove(data.name);
            nameById.Remove(data.id);
        }

        public bool Contains(int id) => ids.Contains(id);

        public string GetNameById(int id) => nameById[id];
        public bool TryGetNameById(int id, out string name) => nameById.TryGetValue(id, out name);
        
        public static IList<ValueDropdownItem<int>> GetValues(IdToName set)
        {
            var list = new ValueDropdownList<int>();

            foreach (var data in set)
            {
                list.Add(data.name, data.id);
            }

            return list;
        }
        
        public static IList<ValueDropdownItem<int>> GetValues(IdToName set, HashSet<int> except)
        {
            var list = new ValueDropdownList<int>();

            foreach (var data in set)
            {
                if (!except.Contains(data.id))
                {
                    list.Add(data.name, data.id);
                }
            }

            return list;
        }
    }
}