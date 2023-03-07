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
        private class CardByName
        {
            [ReadOnly]
            [InfoBox("Not entity name", InfoMessageType.Error, "$" + nameof(isNotEntityError))]
            [InfoBox("Entity name does not exist", InfoMessageType.Error, "$" + nameof(isNotExistEntity))]
            [InfoBox("Card is already added", InfoMessageType.Error, "$" + nameof(isAlreadyAdded))]
            public string name;
            [OnValueChanged("OnValueChanged")]
            public T obj;
            
#if UNITY_EDITOR
            private static HashSet<string> addedNames = new();
            private bool isNotEntityError;
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
                var splited = obj.name.Split('_');
                name = splited[0];
                isNotEntityError = !GameScopes.IsEntityName(name);
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
        public static Dictionary<string, T> ByEntitiesNames { get; } = new(); 
        [SerializeField] private List<CardByName> cardByNames = new ();

        public void Init()
        {
            for (int i = 0; i < cardByNames.Count; i++)
            {
                var data = cardByNames[i];
                ByEntitiesNames.Add(data.name, data.obj);
            }
        }
        
#if UNITY_EDITOR
        [OnInspectorInit]
        private void OnInspectorInit()
        {
            instance = this;
            var guid = AssetDatabase.FindAssets("t: LevelsConfigsManager");
            levelsConfigsManager = AssetDatabase.LoadAssetAtPath<LevelsConfigsManager>(AssetDatabase.GUIDToAssetPath(guid[0]));
        }
#endif
    }
}