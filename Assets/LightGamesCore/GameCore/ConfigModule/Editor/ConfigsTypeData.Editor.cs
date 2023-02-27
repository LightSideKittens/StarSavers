#if UNITY_EDITOR
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Firebase;
using Firebase.Auth;
using Firebase.Extensions;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;
using static Core.ConfigModule.FolderNames;

namespace Core.ConfigModule
{
    public partial class ConfigsTypeData
    {
        
        [MenuItem("Assets/Core/ConfigModule/Update Remote")] 
        static void UpdateRemote()
        {
            FirebaseApp firebaseApp = FirebaseApp.Create(FirebaseApp.DefaultInstance.Options, "FIREBASE_EDITOR"); 
            FirebaseAuth.GetAuth(firebaseApp).SignInAnonymouslyAsync().ContinueWithOnMainThread(task =>
            {
                var fullConfigName = $"{selectedTextAsset.name}{Path.GetExtension(AssetDatabase.GetAssetPath(selectedTextAsset))}";
                var storageRef = Firebase.Storage.FirebaseStorage.GetInstance(firebaseApp).RootReference.Child($"{Configs}/{fullConfigName}");
                storageRef.PutBytesAsync(Encoding.UTF8.GetBytes(selectedTextAsset.text)).ContinueWithOnMainThread(task =>
                {
                    if (task.IsCompletedSuccessfully)
                    {
                        Debug.Log($"Success Update: {selectedTextAsset.name}");
                    }
                    else
                    {
                        Debug.LogError($"Failure Update: {selectedTextAsset.name}. Error: {task.Exception.Message}");
                    }
                });
            });

        }
        
        [MenuItem("Assets/Core/ConfigModule/Update Remote", true)]
        static bool ValidateUpdateRemote()
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

                if (path.EndsWith($".{FileExtensions.Json}"))
                {
                    return path.Contains(Configs);
                }
            }
            
            return selectedTextAsset != null && AssetDatabase.GetAssetPath(selectedTextAsset).Contains($".{FileExtensions.Json}");
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

    }
}
#endif