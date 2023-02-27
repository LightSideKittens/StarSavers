using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Battle.Data
{
    public class GameScopesConverter : JsonConverter<GameScopes.Scope>
    {
        public override void WriteJson(JsonWriter writer, GameScopes.Scope value, JsonSerializer serializer)
        {
            var scope = new JObject();
            RecurWrite(scope, value);
            scope.WriteTo(writer);
        }

        private void RecurWrite(JObject oldObj, GameScopes.Scope value)
        {
            var newObj = new JObject();
            oldObj[value.name] = newObj;
            
            for (int i = 0; i < value.scopes.Count; i++)
            {
                var scope = value.scopes[i];
                RecurWrite(newObj, scope);
            }
        }

        public override GameScopes.Scope ReadJson(JsonReader reader, Type objectType, GameScopes.Scope existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            JObject data = JObject.Load(reader);
            
            var globalScope = new GameScopes.Scope();
            foreach (var prop in data.Properties())
            {
                globalScope.name = prop.Name;
                RecurRead((JObject)prop.Value, globalScope);
            }
            
            return globalScope;
        }
        
        private void RecurRead(JObject oldObj, GameScopes.Scope value)
        {
            foreach (var prop in oldObj.Properties())
            {
                var scope = new GameScopes.Scope
                {
                    name = prop.Name
                };
                value.scopes.Add(scope);
                RecurRead((JObject)prop.Value, scope);
            }
        }

    }
}