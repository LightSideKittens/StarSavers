using System;
using UnityEngine;

namespace Core.SingleService
{
    [DisallowMultipleComponent]
    public abstract class BaseSingleService : MonoBehaviour
    {
        public abstract Type Type { get; }
    }
}