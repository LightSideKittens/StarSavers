using System;
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

        public static bool TryGetCurrent(ref LevelConfig config)
        {
            return config != null || LSPropertyEditor.AllEditors.TryGetInspectedObject(out config);
        }
        
        public static HashSet<int> GetExcept(ref HashSet<int> set, ref LevelConfig config, Action<LevelConfig> onGet)
        {
            set ??= new HashSet<int>();
            set.Clear();
            
            if (TryGetCurrent(ref config))
            {
                onGet(config);
            }

            return set;
        }
#endif
        
        public static int GetLevel(string configName)
        {
            var split = configName.Split('_');
            return int.Parse(split[1]);
        }

        public int Level => GetLevel(name);

        public void Apply()
        {
            var entiProps = EntiProps.ByName;
            var allPropsByEntityId = new Dictionary<int, HashSet<string>>();
            AddProps(EntityUpgrades);

            for (int i = 0; i < OtherUpgrades.Count; i++)
            {
                AddProps(OtherUpgrades[i]);
            }
            
            void AddProps(EntityGameProps props)
            {
                foreach (var id in EntityMeta.GetAllEntityIds(props.Destination))
                {
                    if (!allPropsByEntityId.TryGetValue(id, out var propsDict))
                    {
                        propsDict = new HashSet<string>();
                        allPropsByEntityId.Add(id, propsDict);
                    }

                    foreach (var prop in props.Props)
                    {
                        if (propsDict.Add(prop.Name))
                        {
                            if (!entiProps.TryGetValue(id, out var entiPropsDict))
                            {
                                entiPropsDict = new EntiProps.Props();
                                entiProps.Add(id, entiPropsDict);
                            }
                            
                            if (entiPropsDict.TryGetValue(prop.Name, out var propValue))
                            {
                                entiPropsDict[prop.Name] = prop.Upgrade(propValue);
                            }
                            else
                            {
                                entiPropsDict.Add(prop.Name, Prop.Copy(prop.Prop));
                            }
                        }
                    }
                }
            }
        }
    }
}