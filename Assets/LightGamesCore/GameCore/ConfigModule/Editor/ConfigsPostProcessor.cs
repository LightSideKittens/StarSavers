#if UNITY_EDITOR
using System.IO;
using UnityEditor;

namespace Core.ConfigModule
{
    public class ConfigsPostProcessor : AssetPostprocessor
    {
        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            var json = $".{FileExtensions.Json}";
            
            foreach (string str in importedAssets)
            {
                if (Path.GetExtension(str) == json)
                {
                    ConfigsTypeData.CallLoadOnNextAccess(Path.GetFileNameWithoutExtension(str));
                }
            }
        }
    }
}
#endif