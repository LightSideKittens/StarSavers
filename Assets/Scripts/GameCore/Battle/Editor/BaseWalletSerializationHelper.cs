using System;
using System.Collections.Generic;
using System.Reflection;
using Sirenix.OdinInspector.Editor;
using UnityEditor.Compilation;
using UnityEngine;
using Assembly = UnityEditor.Compilation.Assembly;

public abstract class BaseWalletSerializationHelper<T> : OdinEditor where T : ScriptableObject
{
    protected static List<string> matchedTypesNames = new List<string>();
    protected static List<Type> matchedTypes = new List<Type>();
    protected T config;

    protected abstract void OnInit();
    protected abstract void OnButtonClicked(Type type);

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        Init();

        for (int i = 0; i < matchedTypes.Count; i++)
        {
            if (GUILayout.Button($"Add {matchedTypesNames[i]} Price"))
            {
                OnButtonClicked(matchedTypes[i]);
            }
        }

        serializedObject.ApplyModifiedProperties();
    }

    protected void Init()
    {
        if (matchedTypes.Count == 0)
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
            
            config = (T) target;
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
                            matchedTypesNames.Add(type.Name);
                            matchedTypes.Add(nestedType);
                        }
                    }
                }
            }
        }
    }
}
