using System;
using Core.SingleService;

namespace BeatRoyale.Interfaces
{
    public abstract class BaseInitializer<T> : SingleService<BaseInitializer<T>>
    {
        public static void Initialize(Action onInit) => Instance.Internal_Initialize(onInit);
        protected abstract void Internal_Initialize(Action onInit);
    }
}