using UnityEngine;

namespace LGCore
{
    internal interface IInputProvider
    {
        bool IsTouchDown { get; }
        bool IsTouching { get; }
        bool IsTouchUp { get; }
        Vector3 MousePosition { get; }
    }
}