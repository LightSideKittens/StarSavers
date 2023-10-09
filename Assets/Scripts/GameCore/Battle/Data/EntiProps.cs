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
        public class PropsByEntityId : Dictionary<int, Props> { }

        [Serializable]
        public class Props : Dictionary<string, float>
        {
            public float GetValue<T>() where T : BaseGameProperty
            {
                return this[typeof(T).Name];
            }
        }
        
        [JsonProperty("props")] private PropsByEntityId props = new();

        public static PropsByEntityId ByName => Config.props;
        public static Props GetProps(int entityId) => Config.props[entityId];
        public static void Clear() => Config.props.Clear();
    }
}