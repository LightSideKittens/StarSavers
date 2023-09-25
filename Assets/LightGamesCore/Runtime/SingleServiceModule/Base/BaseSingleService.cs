using System;
using UnityEngine;

namespace LGCore
{
    [DisallowMultipleComponent]
    public abstract class BaseSingleService : MonoBehaviour
    {
        public abstract Type Type { get; }
    }
}