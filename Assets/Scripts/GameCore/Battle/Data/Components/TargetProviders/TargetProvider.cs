using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameCore.Battle.Data.Components.TargetProviders
{
    internal abstract class TargetProvider
    {
        [NonSerialized] public bool isOpponent;
        public abstract IEnumerable<Transform> Targets { get; }
    }
}