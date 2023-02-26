using System;

namespace Core.ConfigModule
{
    [Serializable]
    public abstract class JsonBaseConfigData<T> : BaseConfigData<T> where T : JsonBaseConfigData<T>, new()
    {
        protected internal override string Ext => FileExtensions.Json;
    }
}