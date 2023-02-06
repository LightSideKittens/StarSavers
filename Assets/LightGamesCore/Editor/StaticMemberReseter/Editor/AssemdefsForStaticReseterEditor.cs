using Core;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AssemdefsForStaticReseter))]
public class AssemdefsForStaticReseterEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("Reload Domain"))
        {
            EditorUtility.RequestScriptReload();
        }
    }
}
