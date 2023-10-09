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
        public sealed class Group : IEnumerable<int>
        {
            [HideInInspector] public int id;

            private string Name
            {
                get
                {
                    Instance.groupNames.TryGetNameById(id, out var name);
                    return name;
                }
            }
            
            [Header("$Name")]
            [ValueDropdown(nameof(EntityNames), IsUniqueList = true)] 
            [OdinSerialize] private HashSet<int> entites = new();
            
            [ValueDropdown(nameof(GroupNames), IsUniqueList = true)]
            [OdinSerialize] private HashSet<int> includedGroups = new();
            
            
/*
#if UNITY_EDITOR
            [ValueDropdown(nameof(EntityNames), IsUniqueList = true)] 
            [ShowInInspector] private List<int> allEntities = new();
#endif

            private void IncludedGroupsChanged()
            {
                allEntities.Clear();
                
                if (includedGroups.Contains(name))
                {
                    includedGroups.Remove(name);
                }

                foreach (var group in includedGroups)
                {
                    readOnlyGroups.Add(GroupsByName[group]);
                }
            }*/

            private HashSet<int> ExcludedGroups
            {
                get
                {
                    var set = new HashSet<int>{id};
                    foreach (var group in Instance.groups)
                    {
                        Recur(group, set);
                    }

                    return set;

                    void Recur(Group group, HashSet<int> except)
                    {
                        foreach (var groupId in group.includedGroups)
                        {
                            var newGroup = Instance.groupsByName[groupId];
                            
                            if (groupId == id || newGroup.includedGroups.Contains(id))
                            {
                                except.Add(group.id);
                            }
                            else
                            {
                                Recur(newGroup, except);
                            }
                        }
                    }
                }
            }
            
            private IList<ValueDropdownItem<int>> GroupNames => IdToName.GetValues(EntityMeta.GroupNames, ExcludedGroups);
            private IList<ValueDropdownItem<int>> EntityNames => IdToName.GetValues(EntityMeta.EntityNames);
            public IEnumerator<int> GetEnumerator() => entites.GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            public void Add(Group group) => entites.AddRange(group.entites);

            public override bool Equals(object obj)
            {
                if (obj is Group group)
                {
                    return Equals(group);
                }
                return false;
            }
            
            public bool Equals(Group other)
            {
                return id == other.id;
            }

            public override int GetHashCode()
            {
                return id;
            }
        }

        private static EntityMeta instance;
        
#if UNITY_EDITOR
        private static void TryInitInstance()
        {
            if (instance == null)
            {
                instance = AssetDatabaseUtils.LoadAny<EntityMeta>();
                instance.Init();
            }
        }
#endif

        private static EntityMeta Instance
        {
            get
            {
#if UNITY_EDITOR
                TryInitInstance();
#endif
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
        [ListDrawerSettings(HideAddButton = true, HideRemoveButton = true, DraggableItems = true)]
        [HideReferenceObjectPicker]
        [OdinSerialize] private HashSet<Group> groups = new();
        
        private readonly IdToName allDestinations = new();
        private Dictionary<int, Group> groupsByName;

        public static bool IsEntityName(int name) => EntityNames.Contains(name);
        public static bool IsGroupName(int name) => GroupNames.Contains(name);
        
#if UNITY_EDITOR
        
        [OnInspectorInit]
        private void OnInspectorInit()
        {
            TryInitInstance();
        }
#endif
        
        private void InitData()
        {
            allDestinations.Clear();
            allDestinations.AddRange(entityNames);
            allDestinations.AddRange(groupNames);

            var toRemove = new List<Group>();
            
            foreach (var group in groups)
            {
                if (!groupNames.Contains(group.id))
                {
                    toRemove.Add(group);
                }
            }

            foreach (var remove in toRemove)
            {
                groups.Remove(remove);
            }
            
            foreach (var groupName in groupNames)
            {
                var group = new Group{ id = groupName.id };
                groups.Add(group);
            }
            
            groupsByName = groups.ToDictionary(x => x.id);
        }
        
        public void Init()
        {
            entityNames.Init();
            groupNames.Init();
            entityNames.Changed += InitData;
            groupNames.Changed += InitData;
            InitData();
            instance = this;
        }
    }
}