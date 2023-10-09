using System;
using System.Collections.Generic;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

public abstract class BaseWalletSerializationHelper<T> : OdinEditor where T : ScriptableObject
{
    public static List<string> MatchedTypesNames { get; } = new()
    {
        nameof(Coins),
        nameof(Crystal),
        nameof(Metal),
        nameof(DarkElexir),
    };
    public static List<Type> MatchedTypes { get; } = new()
    {
        typeof(Coins.Price),
        typeof(Crystal.Price),
        typeof(Metal.Price),
        typeof(DarkElexir.Price),
    };

    private static bool isInited;
    protected T config;

    protected abstract void OnInit();
    protected abstract void OnButtonClicked(Type type);
    protected abstract void OnDrawButton(Type type);

    
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        Init();
        EditorGUILayout.Space(20);
        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        for (int i = 0; i < MatchedTypes.Count; i++)
        {
            EditorGUILayout.BeginVertical();
            var type = MatchedTypes[i];
            
            if (GUILayout.Button($"{MatchedTypesNames[i]}", GUILayout.Width(100), GUILayout.Height(100)))
            {
                OnButtonClicked(type);
            }

            OnDrawButton(type);
            
            EditorGUILayout.EndVertical();
        }
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();
        serializedObject.ApplyModifiedProperties();
    }

    protected void Init()
    {
        config = (T) target;
        if (!isInited)
        {
            isInited = true;
            OnInit();
        }
    }
}
