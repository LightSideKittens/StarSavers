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
        protected IEnumerable<Id> Ids => group;
        private bool HideGroup => name.Contains("_Base");
#endif
        
        
        [SerializeField, ValueDropdown("Ids")] private Id id;
        public Dictionary<string, Prop> Props { get; private set; }
        
        public bool IsOpponent { get; private set; }
        public string UserId { get; private set; }
        public new Transform transform { get; private set; }
        
        [ShowIf("$HideGroup")]
        [SerializeField] protected LevelIdGroup group;

        public Id Id => id;

        public float GetValue<T>() where T : BaseGameProperty
        {
            return FloatAndPercent.GetValue<T>(Props);
        }

        public virtual void Init(string userId)
        {
            transform = base.transform;
            UserId = userId;
            IsOpponent = UserId == "Opponent";
            Add(transform, this);
            Props = EntiProps.Get(group.name).GetProps(id);//TODO: Refactor
        }

        public virtual void Destroy()
        {
            Remove(transform);
        }
    }
}