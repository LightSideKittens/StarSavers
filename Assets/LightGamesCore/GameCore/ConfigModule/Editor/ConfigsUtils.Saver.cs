#if UNITY_EDITOR
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using static Core.ConfigModule.FolderNames;

namespace Core.ConfigModule
{
    public static partial class ConfigsUtils
    {
        private static TextAsset selectedTextAsset;
        
        [MenuItem("Assets/ConfigModule/Set As Default")] 
        static void SetAsDefault()
        {
            var assetPath = AssetDatabase.GetAssetPath(selectedTextAsset);

            if (!assetPath.Contains("/Resources/"))
            {
                var path = Regex.Replace(assetPath, SaveData, DefaultSaveData);
                FileInfo file = new FileInfo(path);
                file.Directory.Create();

                File.WriteAllText(file.FullName, selectedTextAsset.text);

                AssetDatabase.Refresh();
            }
            else
            {
                Debug.Log($"Config {selectedTextAsset.name} is already in Resources folder");
            }
        }
        
        [MenuItem("Assets/ConfigModule/Set As Default", true)]
        static bool ValidateSetAsDefault()
        {
            selectedTextAsset = null;
            
            if (Selection.activeObject is TextAsset textAsset)
            {
                selectedTextAsset = textAsset;
                var path = AssetDatabase.GetAssetPath(selectedTextAsset);

                if (path.EndsWith($".{FileExtensions.Json}"))
                {
                    return path.Contains(Configs);
                }
            }
            
            return selectedTextAsset != null && AssetDatabase.GetAssetPath(selectedTextAsset).Contains($".{FileExtensions.Json}");
        }
    }
}
#endif