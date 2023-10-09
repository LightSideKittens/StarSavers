using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GameCore.Battle.Data;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using Sirenix.Utilities;
using UnityEngine;

namespace Battle.Data
{
    public class EntityMeta : SerializedScriptableObject
    {
        [Serializable]
        public class Group : IEnumerable<int>
        {
            [ValueDropdown(nameof(GroupNames))] 
            public int name;
            
            [ValueDropdown(nameof(EntityNames))] 
            [OdinSerialize] private HashSet<int> entites;
            
            [ValueDropdown(nameof(GroupNames))] 
            [OdinSerialize] private HashSet<int> includedGroups;
            
/*#if UNITY_EDITOR
            [ShowInInspector] private List<Group> readOnlyGroups = new();
#endif

            private void IncludedGroupsChanged()
            {
                readOnlyGroups.Clear();
                
                if (includedGroups.Contains(name))
                {
                    includedGroups.Remove(name);
                }

                foreach (var group in includedGroups)
                {
                    readOnlyGroups.Add(GroupsByName[group]);
                }
            }*/
            
            private IList<ValueDropdownItem<int>> GroupNames => IdToName.ValuesFunction(EntityMeta.GroupNames);
            private IList<ValueDropdownItem<int>> EntityNames => IdToName.ValuesFunction(EntityMeta.EntityNames);
            public IEnumerator<int> GetEnumerator() => entites.GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            public void Add(Group group) => entites.AddRange(group.entites);
        }

        private static EntityMeta instance;

        private static EntityMeta Instance
        {
            get
            {
#if UNITY_EDITOR
                if (instance == null)
                {
                    instance = AssetDatabaseUtils.LoadAny<EntityMeta>();
                }
#endif
                instance.Init();
                return instance;
            }
        }

        public static IdToName AllDestinations => Instance.allDestinations;
        public static IdToName EntityNames => Instance.entityNames;
        public static IdToName GroupNames => Instance.groupNames;
        public static Dictionary<int, Group> GroupsByName => Instance.groupsByName;
        
        [Header("Constants")]
        
        [OdinSerialize] private IdToName entityNames;
        [OdinSerialize] private IdToName groupNames;
        
        [Header("Groups")]
        [SerializeField] private List<Group> groups;
        
        private readonly IdToName allDestinations = new();
        private Dictionary<int, Group> groupsByName;

        public static bool IsEntityName(int name) => EntityNames.Contains(name);
        public static bool IsGroupName(int name) => GroupNames.Contains(name);
        
#if UNITY_EDITOR
        private void OnValidate()
        {
            Init();
        }

        [OnInspectorInit]
        private void OnInspectorInit()
        {
            entityNames.Init();
            groupNames.Init();
        }
#endif
        
        public void Init()
        {
            groupsByName = groups.ToDictionary(x => x.name);
            allDestinations.Clear();
            allDestinations.AddRange(entityNames);
            allDestinations.AddRange(groupNames);
            instance = this;
        }
    }
}