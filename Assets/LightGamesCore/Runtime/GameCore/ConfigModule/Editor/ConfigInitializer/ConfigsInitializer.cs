using System;
using System.Collections.Generic;
using System.Reflection;
using Core.ConfigModule.Attributes;
using UnityEditor;
using Assembly = System.Reflection.Assembly;

namespace Core.ConfigModule
{
    public static class ConfigsInitializer
    {
        [MenuItem("Core/ConfigModule/Generate All Configs")]
        public static void GenerateAllConfigs()
        {
            var assembly = Assembly.Load("Core.ConfigModule");
            var assemblyName = assembly.GetName();
            var targetAssemblies = new List<Assembly> {assembly};
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            for (int i = 0; i < AppDomain.CurrentDomain.GetAssemblies().Length; i++)
            {
                var assem = assemblies[i];
                var referenced = assem.GetReferencedAssemblies();

                for (int j = 0; j < referenced.Length; j++)
                {
                    var name = referenced[j];
                    
                    if (assemblyName.Name == name.Name)
                    {
                        targetAssemblies.Add(assem);
                    }
                }
            }

            for (int i = 0; i < targetAssemblies.Count; i++)
            {
                foreach (var type in targetAssemblies[i].GetTypes())
                {
                    if (type.GetCustomAttribute<GeneratableAttribute>() != null)
                    {
                        var baseType = type;
                        var tempBaseType = baseType.BaseType;

                        while (tempBaseType != null)
                        {
                            baseType = tempBaseType;

                            if (tempBaseType.GetGenericTypeDefinition() == typeof(BaseConfig<>))
                            {
                                break;
                            }
                            
                            tempBaseType = tempBaseType.BaseType;
                        }

                        baseType.GetMethod("Generate", BindingFlags.Static | BindingFlags.NonPublic)?.Invoke(null, null);
                    }
                }
            }
            
            AssetDatabase.Refresh();
        }
    }
}