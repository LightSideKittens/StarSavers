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
        public int entityName;
        [OdinSerialize] public EntityGameProps EntityUpgrades { get; private set; } = new();
        
        [OdinSerialize, TableList] 
        public List<AllDestinationsGameProps> OtherUpgrades { get; private set; }

        [OdinSerialize] public List<BasePrice> Prices { get; set; } = new();

        
        public static int GetLevel(string configName)
        {
            var split = configName.Split('_');
            return int.Parse(split[1]);
        }

        public int Level => GetLevel(name);
    }
}