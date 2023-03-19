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
        public static TextAsset SelectedTextAsset { get; private set; }
        
        [MenuItem("Assets/ConfigModule/Set As Default")] 
        static void SetAsDefault()
        {
            var assetPath = AssetDatabase.GetAssetPath(SelectedTextAsset);

            if (!assetPath.Contains("/Resources/"))
            {
                var path = Regex.Replace(assetPath, SaveData, DefaultSaveData);
                FileInfo file = new FileInfo(path);
                file.Directory.Create();

                File.WriteAllText(file.FullName, SelectedTextAsset.text);

                AssetDatabase.Refresh();
            }
            else
            {
                Burger.Log($"Config {SelectedTextAsset.name} is already in Resources folder");
            }
        }
        
        [MenuItem("Assets/ConfigModule/Set As Default", true)]
        public static bool ValidateSetAsDefault()
        {
            SelectedTextAsset = null;
            
            if (Selection.activeObject is TextAsset textAsset)
            {
                SelectedTextAsset = textAsset;
                var path = AssetDatabase.GetAssetPath(SelectedTextAsset);

                if (path.EndsWith($".{FileExtensions.Json}"))
                {
                    return path.Contains(Configs);
                }
            }
            
            return SelectedTextAsset != null && AssetDatabase.GetAssetPath(SelectedTextAsset).Contains($".{FileExtensions.Json}");
        }
    }
}
#endif