#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using Firebase.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace Core.Server
{
    public class ServerWindow : OdinEditorWindow
    {
        private static Color green = new Color(0.35f, 0.85f, 0.29f);
        private static Color red = new Color(0.84f, 0.35f, 0.29f);
        
        private static string[] configNames = new[]
        {
            "unlockedLevels",
            "entitiesProperties",
            "commonPlayerData",
            "cardDecks",
        };

        [MenuItem("Server/Server Window")]
        private static void OpenWindow()
        {
            GetWindow<ServerWindow>().Show();
        }
        
        [TableList]
        [SerializeField]
        private List<PlayerData> data;

        [Button(ButtonSizes.Large)]
        [GUIColor("green")]
        private void FetchAll()
        {
            foreach (var playerData in data)
            {
                playerData.FetchAll();;
            }
        }
        
        [Button(ButtonSizes.Large)]
        [GUIColor("red")]
        private void PushAll()
        {
            foreach (var playerData in data)
            {
                playerData.PushAll();;
            }
        }

        [OnInspectorGUI]
        private void OnGui()
        {
            EditorUtils.SetSirenixButtonWhiteColor();
            for (int i = 0; i < data.Count; i++)
            {
                data[i].OnGUI();
            }
        }

        [Serializable]
        private class PlayerData
        {
            private static Color green = new Color(0.35f, 0.85f, 0.29f);
            private static Color red = new Color(0.84f, 0.35f, 0.29f);
            
            [VerticalGroup("User")]
            [TableColumnWidth(250)]
            public string userId;
            [TableColumnWidth(500)]
            public List<ConfigData> configData;
            
            [VerticalGroup("User")]
            [Button(ButtonSizes.Medium)]
            [GUIColor("green")]
            public void FetchAll()
            {
                foreach (var data in configData)
                {
                    data.Fetch();
                }
            }
        
            [VerticalGroup("User")]
            [Button(ButtonSizes.Medium)]
            [GUIColor("red")]
            public void PushAll()
            {
                foreach (var data in configData)
                {
                    data.Push();
                }
            }
            
            [HideInTables]
            public void OnGUI()
            {
                for (int i = 0; i < configData.Count; i++)
                {
                    configData[i].userId = userId;
                }
            }
        }

        [Serializable]
        private class ConfigData
        {
            private static Color green = new Color(0.35f, 0.85f, 0.29f);
            private static Color red = new Color(0.84f, 0.35f, 0.29f);
            
            [ValueDropdown("names")]
            public string name;
            [TextArea(4, 20)]
            [InfoBox("$errorMessage", InfoMessageType.Error, "isInvalidJson")]
            public string value;
            private static IEnumerable<string> names => configNames;
            private string errorMessage;
            [NonSerialized] public string userId;

            [HorizontalGroup]
            [Button(ButtonSizes.Medium)]
            [GUIColor("green")]
            public void Fetch()
            {
                Admin.SignIn(() =>
                {
                    var storageRef =  Admin.Database
                        .Collection("PlayersData")
                        .Document(userId)
                        .Collection("Data")
                        .Document(name);
                    
                    storageRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
                    {
                        if (task.IsCompletedSuccessfully)
                        {
                            value = (string)task.Result.ToDictionary()[name];
                            value = JToken.Parse(value).ToString(Formatting.Indented);
                            Burger.Log($"Success Fetch {name} from User: {userId}");
                        }
                        else
                        {
                            Burger.Error($"Failure Fetch: {userId}. Error: {task.Exception.Message}");
                        }
                    });
                });
            }
            
            [HorizontalGroup]
            [Button(ButtonSizes.Medium)]
            [GUIColor("red")]
            public void Push()
            {
                if (isInvalidJson)
                {
                    Burger.Error($"[{nameof(ServerWindow)}] Value of config: {name} is invalid JSON format!");
                    return;
                }

                var canPush = EditorUtility.DisplayDialog(
                    $"Pushing", 
                    $"Are you sure you want to push config: {name} to User: {userId}?", 
                    "Yes", 
                    "No");
                
                if(!canPush)
                {return;}
                
                Admin.SignIn(() =>
                {
                    var storageRef =  Admin.Database
                        .Collection("PlayersData")
                        .Document(userId)
                        .Collection("Data")
                        .Document(name);

                    var notFormatedValue = JToken.Parse(value).ToString(Formatting.None);
                    var dict = new Dictionary<string, object>(){{name, notFormatedValue}};
                    storageRef.SetAsync(dict).ContinueWithOnMainThread(task =>
                    {
                        if (task.IsCompletedSuccessfully)
                        {
                            Burger.Log($"Success Push {name} to User: {userId}");
                        }
                        else
                        {
                            Burger.Error($"Failure Push: {userId}. Error: {task.Exception.Message}");
                        }
                    });
                });
            }

            private bool isInvalidJson
            {
                get
                {
                    if (string.IsNullOrEmpty(value))
                    {
                        errorMessage = "Value is empty";
                        return true;
                    }

                    value = value.Trim();

                    if ((value.StartsWith("{") && value.EndsWith("}")) ||
                        (value.StartsWith("[") && value.EndsWith("]")))
                    {
                        try
                        {
                            JToken.Parse(value);
                            return false;
                        }
                        catch (JsonReaderException jex)
                        {
                            errorMessage = jex.Message;
                            return true;
                        }
                        catch (Exception ex)
                        {
                            errorMessage = ex.ToString();
                            return true;
                        }
                    }

                    errorMessage = "Invalid JSON format";
                    return true;
                }
            }
        }
    }
}
#endif