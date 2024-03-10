using System;
using LSCore;

namespace MultiWars.Interfaces
{
    public abstract class BaseInitializer : SingleService<BaseInitializer>
    {
        public static void Initialize(Action onInit) => Instance.Internal_Initialize(onInit);
        protected abstract void Internal_Initialize(Action onInit);
    }
}