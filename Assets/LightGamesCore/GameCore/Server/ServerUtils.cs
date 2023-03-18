#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Core.Server;
using Firebase.Extensions;
using UnityEditor;
using UnityEngine;
using UnityToolbarExtender;
using static Core.ConfigModule.FolderNames;
using static Core.ConfigModule.BaseConfig<Core.ConfigModule.DebugData>;
using static Core.ConfigModule.ConfigsUtils;

namespace Core.ConfigModule
{
    [InitializeOnLoad]
    public static class ServerUtils
    {
        private static readonly List<string> localConfigs = new();
        private static readonly HashSet<string> ignoredConfigs = new()
        {
            "commonPlayerData",
            "configVersions",
            "changedLevels",
        };

        static ServerUtils()
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
            Admin.SignIn(() =>
            {
                var data =  Admin.Firestore
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
            });
        }
        
        
        [MenuItem("Assets/ConfigModule/Update Remote")] 
        static void UpdateRemote()
        {
            Admin.SignIn(() =>
            {
                var fullConfigName = $"{SelectedTextAsset.name}{Path.GetExtension(AssetDatabase.GetAssetPath(SelectedTextAsset))}";
                var storageRef = Admin.Storage.RootReference.Child($"{Configs}/{fullConfigName}");
                storageRef.PutBytesAsync(Encoding.UTF8.GetBytes(SelectedTextAsset.text)).ContinueWithOnMainThread(task =>
                {
                    if (task.IsCompletedSuccessfully)
                    {
                        Debug.Log($"Success Update: {SelectedTextAsset.name}");
                    }
                    else
                    {
                        Debug.LogError($"Failure Update: {SelectedTextAsset.name}. Error: {task.Exception.Message}");
                    }
                });
            });
        }
        
        [MenuItem("Assets/ConfigModule/Update Remote", true)]
        static bool ValidateUpdateRemote()
        {
            ValidateSetAsDefault();
            return SelectedTextAsset != null;
        }
    }
}
#endif