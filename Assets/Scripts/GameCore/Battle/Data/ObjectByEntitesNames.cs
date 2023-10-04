using System;
using System.Collections.Generic;
using Battle.Data;
using Sirenix.OdinInspector;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using Object = UnityEngine.Object;

namespace GameCore.Battle.Data
{
    public class ObjectByEntitesNames<T> : SerializedScriptableObject where T : Object
    {
        [Serializable]
        private class ObjectByName
        {
            private IEnumerable<string> Entities => GameScopes.EntitiesNames;
            [InfoBox("Entity name does not exist in LevelsConfigsManager", InfoMessageType.Error, "$isNotExistEntity")]
            [InfoBox("Name is already added", InfoMessageType.Error, "$isAlreadyAdded")]
            [ValueDropdown(nameof(Entities))]
            public string name;
            [OnValueChanged("OnValueChanged")]
            public T obj;
            
#if UNITY_EDITOR
            private static HashSet<string> addedNames = new();
            private bool isNotExistEntity;
            private bool isAlreadyAdded;

            [OnInspectorInit]
            private void OnInspectorInit()
            {
                if (obj != null)
                {
                    OnValueChanged();
                }
            }
            
            private void OnValueChanged()
            {
                isNotExistEntity = !levelsConfigsManager.EntitesNames.Contains(name);
                addedNames.Clear();
                
                for (int i = 0; i < instance.cardByNames.Count; i++)
                {
                    var data = instance.cardByNames[i];
                    data.isAlreadyAdded = !addedNames.Add(data.name);
                }
            }
#endif
        }

        private static ObjectByEntitesNames<T> instance;
        private static LevelsConfigsManager levelsConfigsManager;
        public static Dictionary<string, T> ByName { get; } = new(); 
        [SerializeField] private List<ObjectByName> cardByNames = new ();

        public void Init()
        {   
            ByName.Clear();
            
            for (int i = 0; i < cardByNames.Count; i++)
            {
                var data = cardByNames[i];
                ByName.TryAdd(data.name, data.obj);
            }
        }
        
#if UNITY_EDITOR
        [OnInspectorInit]
        private void OnInspectorInit()
        {
            instance = this;
            levelsConfigsManager = LevelsConfigsManager.Editor_GetInstance();
        }
#endif
    }
}