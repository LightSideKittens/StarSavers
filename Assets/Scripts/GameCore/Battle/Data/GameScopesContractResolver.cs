using System;
using Newtonsoft.Json.Serialization;

namespace Battle.Data
{
    public class GameScopesContractResolver : DefaultContractResolver
    {
        protected override JsonContract CreateContract(Type objectType)
        {
            JsonContract contract = base.CreateContract(objectType);

            
            if (objectType == typeof(GameScopes.Scope))
            {
                contract.Converter = new GameScopesConverter();
            }
			
            return contract;
        }
    }
}