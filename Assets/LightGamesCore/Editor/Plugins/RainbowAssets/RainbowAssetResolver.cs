using UnityEditor;

[InitializeOnLoad]
public static class RainbowAssetResolver
{
    static RainbowAssetResolver()
    {
        EditorPrefs.SetString("Borodar.RainbowFolders.HomeFolder." +  Borodar.RainbowFolders.ProjectEditorUtility.ProjectName, "Assets/LightGamesCore/Editor/Plugins/RainbowAssets/RainbowFolders/");
    }
}