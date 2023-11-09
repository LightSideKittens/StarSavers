using System.Collections.Generic;
using LSCore;
using LSCore.LevelSystem;
using Sirenix.OdinInspector;
using UnityEngine;
using static Battle.ObjectsByTransfroms<Battle.Data.BaseUnit>;

namespace Battle.Data
{
    public class BaseUnit : SerializedMonoBehaviour
    {
#if UNITY_EDITOR
        protected virtual string GroupName => string.Empty;
        protected IEnumerable<Id> Ids => AssetDatabaseUtils.LoadAny<IdGroup>(GroupName);
#endif
        
        [SerializeField, ValueDropdown("Ids")] private Id id;
        public Dictionary<string, Prop> Props { get; private set; }
        
        public bool IsOpponent { get; private set; }
        public string UserId { get; private set; }

        public Id Id => id;

        public float GetValue<T>() where T : BaseGameProperty
        {
            return FloatAndPercent.GetValue<T>(Props);
        }

        public virtual void Init(string userId)
        {
            UserId = userId;
            IsOpponent = UserId == "Opponent";
            Add(transform, this);
            Props = EntiProps.GetProps(id);
        }

        public virtual void Destroy()
        {
            Remove(transform);
        }
    }
}