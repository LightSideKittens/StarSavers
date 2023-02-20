using System;
using System.Collections.Generic;
using Battle.Data.GameProperty;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Battle.Data
{
    [CreateAssetMenu(fileName = nameof(LevelConfig), menuName = "Battle/" + nameof(LevelConfig), order = 0)]
    public class LevelConfig : SerializedScriptableObject
    {
        [InfoBox("First level should contains entity scope with fixed value at all properties", InfoMessageType.Error, "$" + nameof(isError))]
        [OdinSerialize, TableList] public List<GamePropertiesByScope> UpgradesByScope { get; set; }
        [OdinSerialize] public List<BaseWallet> Price { get; set; }

        public static Dictionary<Type, List<BaseGameProperty>> Properties { get; } = new();
        private bool isError;

        public void InitProperties()
        {
            for (int i = 0; i < UpgradesByScope.Count; i++)
            {
                var upgrade = UpgradesByScope[i];

                for (int j = 0; j < upgrade.Properties.Count; j++)
                {
                    var prop = upgrade.Properties[j];
                    var propType = prop.GetType();
                    prop.scope = upgrade.Scope;

                    if (!Properties.TryGetValue(propType, out var list))
                    {
                        list = new List<BaseGameProperty>();
                        Properties.Add(propType, list);
                    }
                    
                    list.Add(prop);
                }
            }
        }

        [OnInspectorGUI]
        private void OnInspectorGUI()
        {
            var scopeHashSet = new HashSet<string>();
            var splitedName = name.Split('_');

            if (splitedName[^1] == "1")
            {
                var upgrade = UpgradesByScope[0];
                isError = !upgrade.Scope.Contains(splitedName[0]);

                if (!isError)
                {
                    for (int j = 0; j < upgrade.Properties.Count; j++)
                    {
                        var prop = upgrade.Properties[j];

                        if (prop.Fixed == 0)
                        {
                            isError = true;
                            break;
                        }
                    }
                }
            }
    
            for (int i = 0; i < UpgradesByScope.Count; i++)
            {
                var step = UpgradesByScope[i];
                step.isError = !scopeHashSet.Add(step.Scope);
            }

            if (UpgradesByScope.Count > 1)
            {
                UpgradesByScope.Sort((a, b) =>
                {
                    var bLength = b.Scope.Split('/').Length;
                    var aLength = a.Scope.Split('/').Length;
                    return Math.Sign(bLength - aLength);
                });
            }
        }
    }
}