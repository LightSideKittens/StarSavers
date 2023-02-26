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
        protected override JsonSerializerSettings Settings { get; } = new() {ContractResolver = new GameScopesContractResolver()};

        [Serializable]
        public class Scope
        {
            public string name;
            public List<Scope> scopes = new();

            public Scope(string name)
            {
                this.name = name;
            }

            public Scope AddChild(Scope child)
            {
                scopes.Add(child);
                return this;
            }
        }

        public Scope scopes = new("Global");
        private readonly List<string> allScopes = new();
        private readonly List<string> entitiesScopes = new();
        private readonly Dictionary<string, string> entitiesScopesByEntityName = new();

        public static HashSet<string> EntityScopesSet { get; } = new();
        public static HashSet<string> ScopesSet { get; } = new();

        public static IEnumerable<string> Scopes => Config.allScopes;

        public static IEnumerable<string> EntityScopes => Config.entitiesScopes;

        public static string GetEntityNameByScopeName(string scopeName)
        {
            return Config.entitiesScopesByEntityName[scopeName];
        }
        
        public static IEnumerable<string> GetEnitiesNamesByScope(string scope)
        {
            var split = scope.Split('/');
            var currentScope = Config.scopes;
            
            for (int i = 1; i < split.Length; i++)
            {
                var currentScopes = currentScope.scopes;
                
                for (int j = 0; j < currentScopes.Count; j++)
                {
                    if (currentScopes[j].name == split[i])
                    {
                        currentScope = currentScopes[j];
                        break;
                    }
                }
            }

            for (int i = 0; i < currentScope.scopes.Count; i++)
            {
                yield return currentScope.scopes[i].name;
            }
        }

        public void Init(TextAsset json)
        {
            var newScope = JsonConvert.DeserializeObject<Scope>(json.text, this.Settings);
        }

        private void RecurScopes(Scope oldScope, string path)
        {
            var stringBuilder = new StringBuilder(path);
            var scopePath = stringBuilder.ToString();
            allScopes.Add(scopePath);
            ScopesSet.Add(scopePath);
            var scopes = oldScope.scopes;

            if (scopes.Count == 0)
            {
                entitiesScopesByEntityName.Add(oldScope.name, scopePath);
                entitiesScopes.Add(scopePath);
                EntityScopesSet.Add(scopePath);
            }
            
            stringBuilder.Append("/");
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