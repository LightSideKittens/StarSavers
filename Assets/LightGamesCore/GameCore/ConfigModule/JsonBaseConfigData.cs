using System;

namespace Core.ConfigModule
{
    [Serializable]
    public abstract class JsonBaseConfigData<T> : BaseConfigData<T> where T : JsonBaseConfigData<T>, new()
    {
        public override string Ext => FileExtensions.Json;
    }
}