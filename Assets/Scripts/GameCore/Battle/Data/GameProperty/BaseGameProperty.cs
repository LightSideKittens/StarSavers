using System;
using UnityEngine;

namespace Battle.Data.GameProperty
{
    [Serializable]
    public abstract class BaseGameProperty
    {
        [HideInInspector] public string scope;
        public float Fixed;
        [Range(1, 5)] public float Multiply = 1;
        [Range(0, 100)] public int Percent;
    }
}