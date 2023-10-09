using System.Collections.Generic;
using Battle.Data.GameProperty;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using Sirenix.Utilities.Editor;
using UnityEngine;

namespace Battle.Data
{
    [CreateAssetMenu(fileName = nameof(LevelConfig), menuName = "Battle/" + nameof(LevelConfig), order = 0)]
    public class LevelConfig : SerializedScriptableObject
    {
        public int EntityId => EntityUpgrades.Destination;
        
        [HideReferenceObjectPicker]
        [OdinSerialize] public EntityGameProps EntityUpgrades { get; private set; } = new();

        [OdinSerialize]
        [HideReferenceObjectPicker]
        [ListDrawerSettings(HideAddButton = true, OnTitleBarGUI = "OtherUpgradesGui")]
        public List<AllDestinationsGameProps> OtherUpgrades { get; private set; } = new();

        [OdinSerialize] public List<BasePrice> Prices { get; set; } = new();

#if UNITY_EDITOR
        private void OtherUpgradesGui()
        {
            if (SirenixEditorGUI.ToolbarButton(EditorIcons.Plus))
            {
                OtherUpgrades.Add(new AllDestinationsGameProps());
            }
        }
#endif
        
        public static int GetLevel(string configName)
        {
            var split = configName.Split('_');
            return int.Parse(split[1]);
        }

        public int Level => GetLevel(name);
    }
}