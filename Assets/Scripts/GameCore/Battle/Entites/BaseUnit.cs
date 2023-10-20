using System.Collections.Generic;
using Battle.Data;
using Battle.Data.GameProperty;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using static GameCore.Battle.ObjectsByTransfroms<GameCore.Battle.Data.BaseUnit>;

namespace GameCore.Battle.Data
{
    public class BaseUnit : SerializedMonoBehaviour
    {
#if UNITY_EDITOR
        protected virtual IList<ValueDropdownItem<int>> Entities => IdToName.GetValues(EntityMeta.EntityIds);
#endif
        
        [SerializeField, ValueDropdown("Entities")] private int unitName;
        public Dictionary<string, Prop> Props { get; private set; }
        
        public bool IsOpponent { get; private set; }
        public string UserId { get; private set; }

        public int Name => unitName;

        public float GetValue<T>() where T : BaseGameProperty
        {
            return FloatAndPercent.GetValue<T>(Props);
        }

        public virtual void Init(string userId)
        {
            UserId = userId;
            IsOpponent = UserId == "Opponent";
            Add(transform, this);
            Props = EntiProps.GetProps(unitName);
        }

        protected virtual void OnDestroy()
        {
            Remove(transform);
        }
    }
}