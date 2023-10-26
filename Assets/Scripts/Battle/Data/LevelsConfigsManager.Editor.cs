#if UNITY_EDITOR
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEditor;

namespace Battle.Data
{
    public partial class LevelsManager
    {
        private ValueDropdownList<LevelsContainer> list;
        private IList<ValueDropdownItem<LevelsContainer>> AvailableContainer
        {
            get
            {
                if (list == null)
                { 
                    list = new ValueDropdownList<LevelsContainer>();
                    foreach (var id in EntityMeta.EntityIds)
                    {
                        list.Add(id.name, new LevelsContainer(){entityId = id.id});
                    }
                }
                
                return list;
            }
        }
        
        public static LevelsManager Editor_GetInstance()
        {
            var guid = AssetDatabase.FindAssets("t: LevelsManager");
            return AssetDatabase.LoadAssetAtPath<LevelsManager>(AssetDatabase.GUIDToAssetPath(guid[0]));
        }
        
        public static void Editor_RecomputeAllLevels()
        {
            Editor_GetInstance().RecomputeAllLevels();
        }
    }
}
#endif