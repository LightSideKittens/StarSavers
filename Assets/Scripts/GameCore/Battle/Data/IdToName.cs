using System;
using System.Collections.Generic;
using System.Linq;
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

            [HideLabel]
            [LabelWidth(1)]
            [ReadOnly] public int id;
            
            [HideLabel]
            [LabelWidth(1000)]
            [OnValueChanged("NameChanged")] public string name;

            public void Init()
            {
                prevName = name;
            }

            private void NameChanged()
            {
                Undo.ClearAll();
                if (Global.idByName.ContainsKey(name))
                {
                    name = prevName;
                }
                else
                {
                    Global.idByName.Remove(prevName);
                    prevName = name;
                    Global.idByName.Add(prevName, id);
                }
            }
        }

        private static int maxHash;
        public static IdToName Global { get; private set; } = new IdToName();
        private Dictionary<int, string> nameById = new();
        private Dictionary<string, int> idByName = new();

        public HashSet<int> Ids => nameById.Keys.ToHashSet();
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
                nameById.Add(id, name);
                idByName.Add(name, id);
                Global.AddInternal(data);
                Global.nameById.Add(id, name);
                Global.idByName.Add(name, id);
                IncreaseHash(data);
            }
        }
        
        public bool CreateData()
        {
            var hash = maxHash;
            var name = $"Entity Name {hash}";

            if (Global.nameById.TryAdd(hash, name) && Global.idByName.TryAdd(name, hash))
            {
                var data = new Data{ name = name, id = hash };
                InternalAdd(data);
                return true;
            }
            
            Global.nameById.Remove(hash);
            Global.idByName.Remove(name);
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

        private void InternalAdd(Data data)
        {
            data.Init();
            base.Add(data);
            Global.AddInternal(data);
            idByName.Add(data.name, data.id);
            nameById.Add(data.id, data.name);
            IncreaseHash(data);
            Changed?.Invoke();
        }

        private void AddInternal(Data data) => base.Add(data);
        
        public new bool Add(Data data)
        {
            if (Global.nameById.TryAdd(data.id, data.name) && Global.idByName.TryAdd(data.name, data.id))
            {
                InternalAdd(data);
                return true;
            }
            
            Global.nameById.Remove(data.id);
            Global.idByName.Remove(data.name);
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
            Global.Remove(data);
            Changed?.Invoke();
        }
        
        public new void RemoveAt(int index)
        {
            InternalRemove(this[index]);
            base.RemoveAt(index);
            Global.RemoveAt(index);
            Changed?.Invoke();
        }

        public new void Clear()
        {
            ClearMeta();
            base.Clear();
        }

        private void ClearMeta()
        {
            foreach (var id in nameById.Keys)
            {
                Global.nameById.Remove(id);
            }
            
            foreach (var name in idByName.Keys)
            {
                Global.idByName.Remove(name);
            }
            
            nameById.Clear();
            idByName.Clear();
            maxHash = 0;
        }

        private void InternalRemove(Data data)
        {
            nameById.Remove(data.id);
            idByName.Remove(data.name);
            Global.nameById.Remove(data.id);
            Global.idByName.Remove(data.name);
        }

        public bool ContainsId(int id) => nameById.ContainsKey(id);
        public bool ContainsName(string name) => idByName.ContainsKey(name);
        public string GetNameById(int id) => nameById[id];
        public int GetIdByName(string id) => idByName[id];
        public bool TryGetNameById(int id, out string name) => nameById.TryGetValue(id, out name);
        public bool TryGetIdByName(string name, out int id) => idByName.TryGetValue(name, out id);
        
        public static IList<ValueDropdownItem<int>> GetValues(IdToName set)
        {
            var list = new ValueDropdownList<int>();

            foreach (var data in set)
            {
                list.Add(data.name, data.id);
            }

            return list;
        }

        private static ValueDropdownList<int> values = new();
        
        public static IList<ValueDropdownItem<int>> GetValues(IdToName set, int current, HashSet<int> except)
        {
            values.Clear();

            foreach (var data in set)
            {
                if (!except.Contains(data.id) || data.id == current)
                {
                    values.Add(data.name, data.id);
                }
            }

            return values;
        }
    }
}