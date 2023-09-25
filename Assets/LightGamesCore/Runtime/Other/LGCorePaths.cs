using UnityEngine;

public static class LGCorePaths
{
    public const string Root = "LightGamesCore";
    public const string Python = Root + "/" + nameof(Python);
    public const string Runtime = Root + "/" + nameof(Runtime);
    public const string Editor = Root + "/" + nameof(Editor);
    public const string Firebase = Runtime + "/Firebase";
    
    public static class MenuItem
    {
        public const string Root = "LGCore";
        public const string Tools = Root + "/Tools";
    }
    
    public static class Windows
    {
        public const string Root = MenuItem.Root + "/Windows";
        public const string ModulesManager = Root + "/Modules Manager";
        public const string YamlEditor = Root + "/Yaml Editor";
        public const string ThemeEditor = Root + "/Theme Editor";
    }

    public static string ToFull(this string path) => $"{Application.dataPath}/{path}";
}
