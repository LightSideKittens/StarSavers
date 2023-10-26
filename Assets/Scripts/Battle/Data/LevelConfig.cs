using System.Collections.Generic;
using Battle.Data.GameProperty;
using LSCore;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
#if UNITY_EDITOR
using Sirenix.Utilities.Editor;
using UnityEditor;
#endif
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

        [OdinSerialize] [HideReferenceObjectPicker] public Prices Prices { get; set; } = new();

#if UNITY_EDITOR
        private void OtherUpgradesGui()
        {
            if (SirenixEditorGUI.ToolbarButton(EditorIcons.Plus))
            {
                var allDestinationsProps = new AllDestinationsGameProps();
                allDestinationsProps.Editor_SetDestination(-1);
                OtherUpgrades.Add(allDestinationsProps);
            }
        }

        private static LevelConfig currentInspected;
        private static readonly HashSet<int> except = new ();
        
        public static HashSet<int> Filter()
        {
            except.Clear();
            except.Add(currentInspected.EntityId);
            
            foreach (var upgrade in currentInspected.OtherUpgrades)
            {
                except.Add(upgrade.Destination);
            }

            return except;
        }

        [OnInspectorInit] 
        private void OnInit()
        {
            Prices.Init(typeof(Coins), typeof(Keys), typeof(RedGems), typeof(Rank), typeof(Exp));
            OnGui();
        }

        [OnInspectorGUI] private void OnGui() => currentInspected = this;
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