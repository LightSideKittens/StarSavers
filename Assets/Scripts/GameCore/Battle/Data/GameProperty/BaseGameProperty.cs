using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Battle.Data.GameProperty
{
    [Serializable]
    public abstract class BaseGameProperty
    {
        [HideInInspector] public string scope;
        [HideIf("$" + nameof(needHideFixed))]
        public float Fixed;
        [Range(0, 1)] public float Percent;
        [HideInInspector] public bool needHideFixed;
    }
}