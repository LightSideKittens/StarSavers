using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace Core.ConfigModule
{
    public static partial class ConfigsUtils
    {
        private static readonly Dictionary<string, Action> loadOnNextAccessActions = new();

        [Conditional("UNITY_EDITOR")]
        public static void AddLoadOnNextAccessAction(string name, Action action)
        {
            if (Application.isEditor)
            {
                loadOnNextAccessActions.TryAdd(name, action);
            }
        }

        [Conditional("UNITY_EDITOR")]
        public static void CallLoadOnNextAccess(string name)
        {
            if (Application.isEditor)
            {
                loadOnNextAccessActions.TryGetValue(name, out var action);
                action?.Invoke();
            }
        }
    }
}