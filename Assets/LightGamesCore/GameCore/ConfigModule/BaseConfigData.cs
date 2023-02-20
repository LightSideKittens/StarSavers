#if !UNITY_EDITOR
#define NOT_EDITOR
#endif

using System;
using System.Diagnostics;
using System.IO;
using UnityEngine;
using static Core.ConfigModule.FolderNames;

namespace Core.ConfigModule
{
    [Serializable]
    public abstract class BaseConfigData<T> : BaseConfig<T> where T : BaseConfigData<T>, new()
    {
        protected override string FolderPath
        {
            get
            {
                var folderPath = string.Empty;
                GetFolderPath_Editor(ref folderPath);
                GetFolderPath_Runtime(ref folderPath);

                return folderPath;
            }
        }
        
        protected override string FullFileName => Path.Combine(FolderPath, $"{FileName}.{Ext}");
        protected override string FileName { get; set; } = $"{char.ToLower(typeof(T).Name[0])}{typeof(T).Name.Substring(1)}";

        [Conditional("UNITY_EDITOR")]
        private void GetFolderPath_Editor(ref string folderPath)
        {
            folderPath = GetFolderPath(Application.dataPath);
        }
        
        [Conditional("NOT_EDITOR")]
        private void GetFolderPath_Runtime(ref string folderPath)
        {
            folderPath = GetFolderPath(Application.persistentDataPath);
        }
        
        private string GetFolderPath(string dataPath)
        {
            return Path.Combine(dataPath, Configs, SaveData, FolderName);
        }
    }
}