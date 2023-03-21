#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Core.Server;
using Firebase.Extensions;
using UnityEditor;
using UnityEngine;
using static Core.ConfigModule.FolderNames;
using static Core.ConfigModule.ConfigsUtils;

namespace Core.ConfigModule
{
    public static class ServerUtils
    {
        private static readonly List<string> localConfigs = new();
        private static readonly HashSet<string> ignoredConfigs = new()
        {
            "commonPlayerData",
            "configVersions",
            "changedLevels",
        };
        
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
                var data =  Admin.Database
                    .Collection("PlayersData")
                    .Document($"{User.Id}")
                    .Collection("Data");

                GetAllLocalConfigs();
                foreach (var names in localConfigs)
                {
                    var storageRef = data.Document(names);
                
                    storageRef.DeleteAsync().ContinueWithOnMainThread(deleteTask =>
                    {
                        if (deleteTask.IsCompletedSuccessfully)
                        {
                            Burger.Log($"Success Deleted: {names} from User: {User.Id}");
                        }
                        else
                        {
                            Burger.Error($"Failure Deleted: {User.Id}. Error: {deleteTask.Exception.Message}");
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
                        Burger.Log($"Success Update: {SelectedTextAsset.name}");
                    }
                    else
                    {
                        Burger.Error($"Failure Update: {SelectedTextAsset.name}. Error: {task.Exception.Message}");
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