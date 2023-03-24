#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Text;
using Firebase.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace Core.Server
{
    public partial class ServerWindow
    {
        [TabGroup("User Database", order: 1)]
        [TableList]
        [SerializeField]
        private List<PlayerData> data;

        [TabGroup("User Database", order: 1)]
        [Button(ButtonSizes.Large)]
        [GUIColor("green")]
        private void FetchAll()
        {
            foreach (var playerData in data)
            {
                playerData.FetchAll();
                ;
            }
        }

        [TabGroup("User Database", order: 1)]
        [Button(ButtonSizes.Large)]
        [GUIColor("red")]
        private void PushAll()
        {
            foreach (var playerData in data)
            {
                playerData.PushAll();
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

            [TableColumnWidth(350)] public List<ConfigData> configData;

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
                    configData[i].names = configNames;
                }
            }
        }

        [Serializable]
        private class ConfigData
        {
            private static Color green = new Color(0.35f, 0.85f, 0.29f);
            private static Color red = new Color(0.84f, 0.35f, 0.29f);

            [ValueDropdown("names")] public string name;

            [TextArea(4, 20)]
            [InfoBox("$errorMessage", InfoMessageType.Error, "isInvalidJson")]
            public string value;

            [NonSerialized] public string[] names;
            private string errorMessage;
            [NonSerialized] public string userId;
            private bool isUserDatabase => !string.IsNullOrEmpty(userId);

            [HorizontalGroup]
            [Button(ButtonSizes.Medium)]
            [GUIColor("green")]
            public void Fetch()
            {
                EditorUtility.DisplayProgressBar("Fetching...", "Fetching in progress...", 0.5f);
                Admin.SignIn(() =>
                {
                    if (isUserDatabase)
                    {
                        var configRef = Admin.Database
                            .Collection("PlayersData")
                            .Document(userId)
                            .Collection("Data")
                            .Document(name);

                        configRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
                        {
                            EditorUtility.ClearProgressBar();
                            if (task.IsCompletedSuccessfully)
                            {
                                value = (string) task.Result.ToDictionary()[name];
                                value = JToken.Parse(value).ToString(Formatting.Indented);
                                Burger.Log($"Success Fetch {name} from User: {userId}");
                            }
                            else
                            {
                                Burger.Error($"Failure Fetch: {userId}. Error: {task.Exception.Message}");
                            }
                        });
                    }
                    else
                    {
                        var storageRef = Admin.ConfigsRef.Child($"{name}.json");
                        storageRef.GetBytesAsync(User.MaxAllowedSize).ContinueWithOnMainThread(task =>
                        {
                            EditorUtility.ClearProgressBar();
                            if (task.IsCompletedSuccessfully)
                            {                                
                                value = Encoding.UTF8.GetString(task.Result);
                                value = JToken.Parse(value).ToString(Formatting.Indented);
                                Burger.Log($"Success Fetch {name} from Storage");
                            }
                            else
                            {
                                Burger.Error($"Failure Fetch: {name}. Error: {task.Exception.Message}");
                            }
                        });
                    }
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
                    isUserDatabase 
                        ? $"Push config: {name} to User: {userId}?" 
                        : $"Push config: {name} to Storage",
                    "Yes",
                    "No");

                if (!canPush)
                {
                    return;
                }

                EditorUtility.DisplayProgressBar("Pushing...", "Pushing in progress...", 0.5f);
                Admin.SignIn(() =>
                {
                    if (isUserDatabase)
                    {
                        var configRef = Admin.Database
                            .Collection("PlayersData")
                            .Document(userId)
                            .Collection("Data")
                            .Document(name);

                        var notFormatedValue = JToken.Parse(value).ToString(Formatting.None);
                        var dict = new Dictionary<string, object>() {{name, notFormatedValue}};
                        configRef.SetAsync(dict).ContinueWithOnMainThread(task =>
                        {
                            EditorUtility.ClearProgressBar();
                            if (task.IsCompletedSuccessfully)
                            {
                                Burger.Log($"Success Push {name} to User: {userId}");
                            }
                            else
                            {
                                Burger.Error($"Failure Push: {userId}. Error: {task.Exception.Message}");
                            }
                        });
                    }
                    else
                    {
                        var storageRef = Admin.ConfigsRef.Child($"{name}.json");
                        var notFormatedValue = JToken.Parse(value).ToString(Formatting.None);
                        storageRef.PutBytesAsync(Encoding.UTF8.GetBytes(notFormatedValue)).ContinueWithOnMainThread(task =>
                        {
                            EditorUtility.ClearProgressBar();
                            if (task.IsCompletedSuccessfully)
                            {
                                Burger.Log($"Success Push {name} to Storage");
                            }
                            else
                            {
                                Burger.Error($"Failure Push: {name}. Error: {task.Exception.Message}");
                            }
                        });
                    }
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