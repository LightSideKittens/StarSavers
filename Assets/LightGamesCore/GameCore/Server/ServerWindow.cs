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
    public partial class ServerWindow : OdinEditorWindow
    {
        private static Color green = new Color(0.35f, 0.85f, 0.29f);
        private static Color red = new Color(0.84f, 0.35f, 0.29f);
        private static bool isButtonStyleSetted;

        private static string[] configNames = new[]
        {
            "unlockedLevels",
            "entiProps",
            "user",
            "cardDecks",
        };

        private static string[] storageConfigNames = new[]
        {
            "unlockedLevels",
            "entiProps",
            "configVersions",
            "cardDecks",
        };

        [MenuItem("Server/Server Window")]
        private static void OpenWindow()
        {
            GetWindow<ServerWindow>().Show();
        }

        [TabGroup("Storage", order: 2)]
        [SerializeField]
        private List<ConfigData> storageConfigs;

        [OnInspectorGUI]
        private void OnGui()
        {
            if (!isButtonStyleSetted)
            {
                EditorUtils.SetSirenixButtonWhiteColor();
                isButtonStyleSetted = true;
            }
            
            data ??= new List<PlayerData>();
            storageConfigs ??= new List<ConfigData>();
            
            for (int i = 0; i < data.Count; i++)
            {
                data[i].OnGUI();
            }
            
            for (int i = 0; i < storageConfigs.Count; i++)
            {
                storageConfigs[i].names = storageConfigNames;
            }
        }
    }
}
#endif