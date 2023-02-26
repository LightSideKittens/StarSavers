using System;

namespace Core.ConfigModule
{
    public abstract class BaseRemoteConfig
    {
        protected abstract void Internal_Push(Action onSuccess = null, Action onError = null);
        protected abstract void Internal_Fetch(Action onSuccess = null, Action onError = null, Action onComplete = null);
    }
}