using System.Collections.Generic;
using Battle.Data;
using Battle.Data.GameProperty;
using Sirenix.OdinInspector;
using UnityEngine;
using static GameCore.Battle.ObjectsByTransfroms<GameCore.Battle.Data.BaseUnit>;

namespace GameCore.Battle.Data
{
    public class BaseUnit : SerializedMonoBehaviour
    {
        [SerializeField, ValueDropdown(nameof(Entities))] private int unitName;
        public Dictionary<string, string> properties;
        
        public bool IsOpponent { get; private set; }
        public string UserId { get; private set; }
        protected virtual IList<ValueDropdownItem<int>> Entities => IdToName.GetValues(EntityMeta.EntityIds);

        public int Name => unitName;
        
        public float GetValue<T>() where T : BaseGameProperty
        {
            return properties[typeof(T).Name].GetFloat();
        }

        public virtual void Init(string userId)
        {
            UserId = userId;
            IsOpponent = UserId == "Opponent";
            Add(transform, this);
            properties = EntiProps.GetProps(unitName);
        }

        protected virtual void OnDestroy()
        {
            Remove(transform);
        }
    }
}