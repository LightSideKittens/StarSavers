#if UNITY_EDITOR
using System.IO;
using UnityEditor;

namespace Core.ConfigModule
{
    public partial class ConfigsTypeData
    {
        [MenuItem("Core/ConfigModule/Clear All Configs Data")]
        private static void ClearAll()
        {
            var paths = Config.paths;

            foreach (var path in paths)
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
            }
            
            AssetDatabase.Refresh();
        }
    }
}
#endif