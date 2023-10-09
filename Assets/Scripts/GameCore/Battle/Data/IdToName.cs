using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace GameCore.Battle.Data
{
    [Serializable]
    public class IdToName : List<IdToName.Data>
    {
        [Serializable]
        public class Data
        {
            [NonSerialized] private string prevName;

            [ReadOnly] public int id;
            [OnValueChanged("NameChanged")] public string name;

            public void Init()
            {
                prevName = name;
            }

            private void NameChanged()
            {
                Undo.ClearAll();
                if (names.Contains(name))
                {
                    name = prevName;
                }
                else
                {
                    names.Remove(prevName);
                    prevName = name;
                    names.Add(prevName);
                    nameById[id] = prevName;
                }
            }
        }

        private static int maxHash;
        private static HashSet<int> ids = new();
        private static HashSet<string> names = new();
        private static Dictionary<int, string> nameById = new();

        public event Action Changed;

        public IdToName()
        {
            Init();
        }
        
        public void Init()
        {
            ClearMeta();
            
            foreach (var data in this)
            {
                var id = data.id;
                var name = data.name;
                data.Init();
                ids.Add(id);
                names.Add(name);
                nameById.Add(id, name);
                IncreaseHash(data);
            }
        }
        
        public bool CreateData()
        {
            var hash = maxHash;
            var name = $"Entity Name {hash}";

            if (ids.Add(hash) && names.Add(name))
            {
                var data = new Data{ name = $"Entity Name {hash}", id = hash };
                data.Init();
                base.Add(data);
                nameById.Add(data.id, data.name);
                IncreaseHash(data);
                Changed?.Invoke();
                return true;
            }
            
            ids.Remove(hash);
            names.Remove(name);
            return false;
        }

        public new void AddRange(IEnumerable<Data> data)
        {
            foreach (var data1 in data)
            {
                if (!Add(data1))
                {
                    throw new Exception($"Detected the same Id: {data1.id} or Name: {data1.name}");
                }
            }
        }

        public new bool Add(Data data)
        {
            if (ids.Add(data.id) && names.Add(data.name))
            {
                data.Init();
                base.Add(data);
                nameById.Add(data.id, data.name);
                IncreaseHash(data);
                Changed?.Invoke();
                return true;
            }
            
            ids.Remove(data.id);
            names.Remove(data.name);
            return false;
        }

        private void IncreaseHash(Data data)
        {
            if (data.id >= maxHash)
            {
                maxHash = data.id + 1;
            }
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

        public new void Clear()
        {
            ClearMeta();
            base.Clear();
        }

        private void ClearMeta()
        {
            ids.Clear();
            names.Clear();
            nameById.Clear();
            maxHash = 0;
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