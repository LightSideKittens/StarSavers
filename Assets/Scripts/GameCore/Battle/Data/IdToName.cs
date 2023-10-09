using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
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
                    prevId = id;
                    set.ids.Add(prevId);
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
                }
            }
        }

        private HashSet<int> ids = new();
        private HashSet<string> names = new();
        
        public void Init()
        {
            foreach (var data1 in this)
            {
                data1.Init(this);
                ids.Add(data1.id);
                names.Add(data1.name);
            }
        }

        public void AddData()
        {
            var hash = Random.Range(-999999, 999999);
            var name = $"Entity Name {hash}";
            Data data = null;

            if (ids.Add(hash) && names.Add(name))
            {
                data = new Data() { name = $"Entity Name {hash}", id = hash };
                data.Init(this);
            }

            Add(data);
        }

        public bool Contains(int id) => ids.Contains(id);
        
        public static IList<ValueDropdownItem<int>> ValuesFunction(IdToName set)
        {
            var list = new ValueDropdownList<int>();

            foreach (var id in set)
            {
                list.Add(id.name, id.id);
            }

            return list;
        }
    }
}