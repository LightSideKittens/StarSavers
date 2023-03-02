using System;
using System.Collections.Generic;
using System.Reflection;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;
using Assembly = UnityEditor.Compilation.Assembly;

public abstract class BaseWalletSerializationHelper<T> : OdinEditor where T : ScriptableObject
{
    public static List<string> MatchedTypesNames { get; } = new List<string>();
    public static List<Type> MatchedTypes { get; } = new List<Type>();
    protected T config;

    protected abstract void OnInit();
    protected abstract void OnButtonClicked(Type type);
    protected abstract void OnDrawButton(Type type);

    
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        Init();

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
        if (MatchedTypes.Count == 0)
        {
            var targetAssembly = System.Reflection.Assembly.GetAssembly(typeof(BaseWallet));
            var playerAssemblies = CompilationPipeline.GetAssemblies(AssembliesType.PlayerWithoutTestAssemblies);
            var matchedAssemblies = new List<Assembly>();

            for (int i = 0; i < playerAssemblies.Length; i++)
            {
                var currentAssembly = playerAssemblies[i];
                var references = currentAssembly.assemblyReferences;

                for (int j = 0; j < references.Length; j++)
                {
                    if (references[j].name == targetAssembly.GetName().Name)
                    {
                        matchedAssemblies.Add(currentAssembly);
                    }
                }
            }
            
            OnInit();

            for (int i = 0; i < matchedAssemblies.Count; i++)
            {
                var types = System.Reflection.Assembly.LoadFrom(matchedAssemblies[i].outputPath).GetTypes();

                for (int j = 0; j < types.Length; j++)
                {
                    var type = types[j];
                    var nestedTypes = type.BaseType.GetNestedTypes(BindingFlags.Public);

                    for (int k = 0; k < nestedTypes.Length; k++)
                    {
                        var nestedType = nestedTypes[k];

                        if (nestedType.IsGenericTypeDefinition)
                        {
                            nestedType = nestedType.MakeGenericType(type);
                        }

                        if (typeof(BaseWallet).IsAssignableFrom(nestedType))
                        {
                            MatchedTypesNames.Add(type.Name);
                            MatchedTypes.Add(nestedType);
                        }
                    }
                }
            }
        }
    }
}
