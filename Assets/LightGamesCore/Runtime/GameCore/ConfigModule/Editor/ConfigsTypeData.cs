using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;
using static Core.ConfigModule.FileExtensions;
using static Core.ConfigModule.FolderNames;

namespace Core.ConfigModule
{
    public class ConfigsTypeData : JsonBaseConfigData<ConfigsTypeData>
    {
        protected override string FolderName => Path.Combine("EditorConfigs", GetType().Name);
        [JsonProperty] private readonly HashSet<string> paths = new HashSet<string>();
        private static TextAsset selectedTextAsset;
        
        public static void Init()
        {
            Initialize();
        }
        
        public new static void Save()
        {
            Set(instance);
        }
        
        [Conditional("UNITY_EDITOR")]
        public static void AddPath(string path)
        {
            Config.paths.Add(path);
        }

#if UNITY_EDITOR
        
        [MenuItem("Assets/Core/ConfigModule/Set As Default")] 
        static void SetAsDefault()
        {
            var assetPath = AssetDatabase.GetAssetPath(selectedTextAsset);
            var path = Regex.Replace(assetPath, SaveData, DefaultSaveData);
            FileInfo file = new FileInfo(path);
            file.Directory.Create();

            File.WriteAllText(file.FullName, selectedTextAsset.text);

            AssetDatabase.Refresh();
        }
        
        [MenuItem("Assets/Core/ConfigModule/Set As Default", true)]
        static bool ValidateSetAsDefault()
        {
            selectedTextAsset = null;
            
            if (Selection.activeObject is TextAsset textAsset)
            {
                selectedTextAsset = textAsset;
                var path = AssetDatabase.GetAssetPath(selectedTextAsset);

                if (path.EndsWith(".json"))
                {
                    return path.Contains("Configs");
                }
            }
            
            return selectedTextAsset != null && AssetDatabase.GetAssetPath(selectedTextAsset).Contains(".json");
        }
        
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
#endif
    }
}