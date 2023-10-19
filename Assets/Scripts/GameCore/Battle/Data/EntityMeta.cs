using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GameCore.Battle.Data;
using LSCore;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using Sirenix.Utilities;
using UnityEditor;
using UnityEngine;

namespace Battle.Data
{
    [InitializeOnLoad]
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
                    Instance.groupIds.TryGetNameById(id, out var name);
                    return name;
                }
            }
            
            [Header("$Name")]
            [ValueDropdown("EntityIds", IsUniqueList = true)] 
            [OdinSerialize] private HashSet<int> entityIds = new();
            
            [ValueDropdown("GroupIds", IsUniqueList = true)]
            [OdinSerialize] private HashSet<int> includedGroups = new();

            private HashSet<Group> ToGroups(IEnumerable<int> groupsIds)
            {
                var set = new HashSet<Group>();

                foreach (var groupsId in groupsIds)
                {
                    set.Add(GroupsById[groupsId]);
                }

                return set;
            }
            
            private HashSet<Group> AllIncludedGroups
            {
                get
                {
                    var set = new HashSet<Group>();
                    Recur(includedGroups, set);

                    return set;

                    void Recur(HashSet<int> groupIds, HashSet<Group> result)
                    {
                        foreach (var newGroup in ToGroups(groupIds))
                        {
                            result.Add(newGroup);
                            Recur(newGroup.includedGroups, result);
                        }
                    }
                }
            }
            public HashSet<int> AllEntityIds
            {
                get
                {
                    var set = new HashSet<int>(entityIds);
                    
                    foreach (var group in AllIncludedGroups)
                    {
                        set.AddRange(group.entityIds);
                    }

                    return set;
                }
            }

#if UNITY_EDITOR
            private HashSet<int> ExcludedGroups
            {
                get
                {
                    var set = new HashSet<int>{id};
                    
                    foreach (var group in Instance.groups)
                    {
                        Exclude(group);
                        
                        foreach (var childGroup in group.AllIncludedGroups)
                        {
                            if (Exclude(childGroup))
                            {
                                set.Add(group.id);
                            }
                        }
                    }

                    bool Exclude(Group group)
                    {
                        if (group.id == id || group.includedGroups.Contains(id))
                        {
                            set.Add(group.id);
                            return true;
                        }

                        return false;
                    }
                    
                    return set;
                }
            }
            
            private HashSet<int> ExcludedEntities
            {
                get
                {
                    var set = new HashSet<int>();
                    
                    foreach (var group in AllIncludedGroups)
                    {
                        set.AddRange(group.entityIds);
                    }

                    return set;
                }
            }
            
            private IList<ValueDropdownItem<int>> GroupIds => IdToName.GetValues(EntityMeta.GroupIds, -1, ExcludedGroups);
            private IList<ValueDropdownItem<int>> EntityIds => IdToName.GetValues(EntityMeta.EntityIds,-1, ExcludedEntities);
#endif
            
            public IEnumerator<int> GetEnumerator() => entityIds.GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            public void Add(Group group) => entityIds.AddRange(group.entityIds);

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
                if (!World.IsPlaying)
                {
                    TryInitInstance();
                }
#endif
                return instance;
            }
        }

        public static IdToName AllDestinations => IdToName.Global;
        public static IdToName EntityIds => Instance.entityIds;
        public static IdToName GroupIds => Instance.groupIds;
        public static Dictionary<int, Group> GroupsById => Instance.groupsById;
        public static Group GetGroupByName(string name) => GroupsById[GroupIds.GetIdByName(name)];
        
        [Header("Constants")]
        [OdinSerialize] private IdToName entityIds;
        [OdinSerialize] private IdToName groupIds;
        
        [Header("Groups")]
        [ListDrawerSettings(HideAddButton = true, HideRemoveButton = true, DraggableItems = true)]
        [HideReferenceObjectPicker]
        [OdinSerialize] private HashSet<Group> groups = new();
        
        private Dictionary<int, Group> groupsById;

        public static bool IsEntityId(int name) => EntityIds.ContainsId(name);
        public static bool IsGroupName(int name) => GroupIds.ContainsId(name);

        public static IEnumerable<int> GetAllEntityIds(int destinationId)
        {
            if (IsEntityId(destinationId))
            {
                yield return destinationId;
            }
            else if(IsGroupName(destinationId))
            {
                foreach (var id in GroupsById[destinationId].AllEntityIds)
                {
                    yield return id;
                }
            }
        }
        
#if UNITY_EDITOR
        
        [OnInspectorInit]
        private void OnInspectorInit()
        {
            TryInitInstance();
        }
#endif
        
        private void InitData()
        {
            var toRemove = new List<Group>();
            
            foreach (var group in groups)
            {
                if (!groupIds.ContainsId(group.id))
                {
                    toRemove.Add(group);
                }
            }

            foreach (var remove in toRemove)
            {
                groups.Remove(remove);
            }
            
            foreach (var groupName in groupIds)
            {
                var group = new Group{ id = groupName.id };
                groups.Add(group);
            }
            
            groupsById = groups.ToDictionary(x => x.id);
            EditorUtility.SetDirty(this);
        }
        
        public void Init()
        {
            entityIds.Init();
            groupIds.Init();
            entityIds.Changed += InitData;
            groupIds.Changed += InitData;
            InitData();
            
            instance = this;
        }
    }
}