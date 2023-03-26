using System.Collections.Generic;
using Battle.Data;
using BeatRoyale;
using Core.Server;
using Sirenix.OdinInspector;
using UnityEngine;
using static GameCore.Battle.ObjectsByTransfroms<GameCore.Battle.Data.BaseEntity>;

namespace GameCore.Battle.Data
{
    public class BaseEntity : SerializedMonoBehaviour
    {
        public bool IsOpponent { get; private set; }
        public string UserId { get; private set; }
        protected virtual IEnumerable<string> Entities => GameScopes.EntitiesNames;
        
        [SerializeField, ValueDropdown(nameof(Entities))] private string entityName;
        
        public static Dictionary<string, ValuePercent> GetProperties(Transform transform)
        {
            var unit = Get(transform);
            return MatchData.GetProperties(unit.UserId).byName[unit.entityName];
        }

        public virtual void Init(string userId)
        {
            UserId = userId;
            IsOpponent = UserId != User.Id;
            Add(transform, this);
        }

        protected virtual void OnDestroy()
        {
            Remove(transform);
        }
    }
}