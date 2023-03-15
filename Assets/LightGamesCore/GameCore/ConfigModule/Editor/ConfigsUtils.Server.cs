#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using BeatRoyale;
using Firebase;
using Firebase.Auth;
using Firebase.Extensions;
using Firebase.Firestore;
using UnityEditor;
using UnityEngine;
using UnityToolbarExtender;
using static Core.ConfigModule.FileExtensions;
using static Core.ConfigModule.FolderNames;
using static Core.ConfigModule.BaseConfig<BeatRoyale.DebugData>;

namespace Core.ConfigModule
{
    [InitializeOnLoad]
    public static partial class ConfigsUtils
    {
        private static readonly List<string> localConfigs = new();
        private static readonly HashSet<string> ignoredConfigs = new()
        {
            "commonPlayerData",
            "configVersions",
            "changedLevels",
        };

        static ConfigsUtils()
        {
            ToolbarExtender.LeftToolbarGUI.Add(OnToolbarLeftGUI);
        }

        private static void OnToolbarLeftGUI()
        {
            var serverEnabled = Config.serverEnabled;
            
            if (serverEnabled != GUILayout.Toggle(serverEnabled, "Enable Server", GUILayout.MaxWidth(100)))
            {
                Config.serverEnabled = !serverEnabled;
                Save();
            }
        }

        private static void GetAllLocalConfigs()
        {
            var files = Directory.GetFiles($"{Application.dataPath}/{Configs}/{SaveData}", "*.json");
            localConfigs.Clear();

            for (int i = 0; i < files.Length; i++)
            {
                var match = Regex.Match(files[i], @"(\w+)\.json");
                var fileName = match.Groups[1].Value;
                
                if (!ignoredConfigs.Contains(fileName))
                {
                    localConfigs.Add(fileName);
                }
            }
        }

        [MenuItem("Server/Delete Player Data")]
        static void DeletePlayerData()
        {
            FirebaseApp firebaseApp = FirebaseApp.Create(FirebaseApp.DefaultInstance.Options, "FIREBASE_EDITOR"); 
            FirebaseAuth.GetAuth(firebaseApp).SignInWithEmailAndPasswordAsync("firebase.admin@beatroyale.com", "firebaseadminbeatroyale")
                .ContinueWithOnMainThread(task =>
            {
                if (task.IsCompletedSuccessfully)
                {
                    var data =  FirebaseFirestore.GetInstance(firebaseApp)
                        .Collection("PlayersData")
                        .Document($"{CommonPlayerData.UserId}")
                        .Collection("Data");

                    GetAllLocalConfigs();
                    foreach (var names in localConfigs)
                    {
                        var storageRef = data.Document(names);
                
                        storageRef.DeleteAsync().ContinueWithOnMainThread(deleteTask =>
                        {
                            if (deleteTask.IsCompletedSuccessfully)
                            {
                                Debug.Log($"Success Deleted: {names} from User: {CommonPlayerData.UserId}");
                            }
                            else
                            {
                                Debug.LogError($"Failure Deleted: {CommonPlayerData.UserId}. Error: {deleteTask.Exception.Message}");
                            }
                        });
                    }
                }
                else
                {
                    Debug.LogError($"Failure Deleted: {CommonPlayerData.UserId}. Error: {task.Exception.Message}");
                }
            });

        }
        
        
        [MenuItem("Assets/ConfigModule/Update Remote")] 
        static void UpdateRemote()
        {
            FirebaseApp firebaseApp = FirebaseApp.Create(FirebaseApp.DefaultInstance.Options, "FIREBASE_EDITOR"); 
            FirebaseAuth.GetAuth(firebaseApp).SignInWithEmailAndPasswordAsync("firebase.admin@beatroyale.com", "firebaseadminbeatroyale").ContinueWithOnMainThread(task =>
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
        
        [MenuItem("Assets/ConfigModule/Update Remote", true)]
        static bool ValidateUpdateRemote()
        {
            selectedTextAsset = null;
            
            if (Selection.activeObject is TextAsset textAsset)
            {
                selectedTextAsset = textAsset;
                var path = AssetDatabase.GetAssetPath(selectedTextAsset);

                if (path.EndsWith($".{Json}"))
                {
                    return path.Contains(Configs);
                }
            }
            
            return selectedTextAsset != null && AssetDatabase.GetAssetPath(selectedTextAsset).Contains($".{Json}");
        }
    }
}
#endif