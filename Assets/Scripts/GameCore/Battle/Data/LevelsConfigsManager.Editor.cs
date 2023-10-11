#if UNITY_EDITOR
using UnityEditor;

namespace Battle.Data
{
    public partial class LevelsManager
    {
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