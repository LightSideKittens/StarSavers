using System;
using System.Collections.Generic;
using System.Text;
using Core.ConfigModule;
using Newtonsoft.Json;
using UnityEngine;

namespace Battle.Data
{
    public class GameScopes : JsonBaseConfigData<GameScopes>
    {
        protected override bool NeedAutoSave => false;
        protected override JsonSerializerSettings Settings { get; } = new() {ContractResolver = new GameScopesContractResolver()};

        [Serializable]
        public class Scope
        {
            public string name;
            public List<Scope> scopes = new();
        }

        [JsonProperty] private readonly Scope scopes = new(){name = "Global"};
        private readonly Dictionary<string, Scope> allScopes = new();
        private readonly Dictionary<string, HashSet<string>> scopesByEntityName = new();
        private readonly Dictionary<string, string> entityNameByEntityScope = new();
        private readonly HashSet<string> heroesNames = new();

        public static IEnumerable<string> Scopes => Config.allScopes.Keys;
        public static IEnumerable<string> EntitiesNames => Config.scopesByEntityName.Keys;
        public static IEnumerable<string> HeroesNames => Config.heroesNames;
        public static bool IsEntityName(string entityName) => Config.scopesByEntityName.ContainsKey(entityName);
        public static bool IsEntityScope(string entityScope) => Config.entityNameByEntityScope.ContainsKey(entityScope);

        public static bool TryGetEntityNameFromScope(string scope, out string entityName)
        {
            if (IsEntityScope(scope))
            {
                entityName = Config.entityNameByEntityScope[scope];
                return true;
            }

            entityName = null;
            return false;
        }
        public static bool Contains(string entitesName, string scope) => Config.scopesByEntityName[entitesName].Contains(scope);

        public static IEnumerable<string> GetEnitiesNamesByScope(string scope)
        {
            if (IsEntityScope(scope))
            {
               yield return Config.entityNameByEntityScope[scope];
            }
            else
            {
                var currentScope = Config.allScopes[scope];
                var entitesNames = new List<string>();
                Recur(currentScope);

                for (int i = 0; i < entitesNames.Count; i++)
                {
                    yield return entitesNames[i];
                }

                void Recur(Scope curScope)
                {
                    var scopes = curScope.scopes;

                    if (scopes.Count == 0)
                    {
                        entitesNames.Add(curScope.name);
                    }
                    
                    for (int i = 0; i < scopes.Count; i++)
                    {
                        Recur(scopes[i]);
                    }
                }
            }
        }

        private void RecurScopes(Scope oldScope, string path)
        {
            var stringBuilder = new StringBuilder(path);
            var scopePath = stringBuilder.ToString();
            allScopes.Add(scopePath, oldScope);
            var scopes = oldScope.scopes;

            if (scopes.Count == 0)
            {
                var scopesSet = new HashSet<string>();
                scopesByEntityName.Add(oldScope.name, scopesSet);
                entityNameByEntityScope.Add(scopePath, oldScope.name);
                var split = scopePath.Split('/');
                var builder = new StringBuilder();

                if (split[1] == "Heroes")
                {
                    heroesNames.Add(oldScope.name);
                }
                
                for (int i = 0; i < split.Length; i++)
                {
                    var scopeName = split[i];
                    builder.Append(scopeName);
                    scopesSet.Add(builder.ToString());
                    builder.Append('/');
                }
            }
            
            stringBuilder.Append('/');
            for (int i = 0; i < scopes.Count; i++)
            {
                var scope = scopes[i];
                RecurScopes(scope, $"{stringBuilder.ToString()}{scope.name}");
            }
        }
        
        protected override void OnLoaded()
        {
            base.OnLoaded();
            RecurScopes(scopes, scopes.name);
        }
    }
}