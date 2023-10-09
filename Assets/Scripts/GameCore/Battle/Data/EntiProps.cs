using System;
using System.Collections.Generic;
using Battle.Data.GameProperty;
using LSCore.ConfigModule;
using Newtonsoft.Json;

namespace Battle.Data
{
    public class EntiProps : BaseConfig<EntiProps>
    {
        [Serializable]
        public class PropsByEntityName : Dictionary<int, Props> { }

        [Serializable]
        public class Props : Dictionary<string, float>
        {
            public float GetValue<T>() where T : BaseGameProperty
            {
                return this[typeof(T).Name];
            }
        }
        
        [JsonProperty("props")] private PropsByEntityName props = new();

        public static PropsByEntityName ByName => Config.props;
        public static Props GetProps(int entityName) => Config.props[entityName];
        public static void Clear() => Config.props.Clear();
    }
}