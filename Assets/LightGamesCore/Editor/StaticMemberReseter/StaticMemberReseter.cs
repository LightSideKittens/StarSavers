using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using Core.ConfigModule;
using Newtonsoft.Json;
using Polenter.Serialization;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Core
{
    public static class StaticMemberReseter
    {
        private struct MemberData
        {
            public string json;
            public MemoryStream stream;
            public FieldInfo fieldInfo;
        }
        
        private static readonly Dictionary<Type, List<MemberData>> types = new Dictionary<Type, List<MemberData>>();
        private static readonly HashSet<Type> checkedTypes = new HashSet<Type>();
        
        private static readonly JsonSerializerSettings settings = new JsonSerializerSettings()
        {
            ContractResolver = UnityJsonContractResolver.Instance
        };

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Reset()
        {
            if (!EditorSettings.enterPlayModeOptionsEnabled)
            {
                return;
            }

            if (!EditorSettings.enterPlayModeOptions.HasFlag(EnterPlayModeOptions.DisableDomainReload))
            {
                return;
            }

            var assemdefCollection = Resources.Load<AssemdefsForStaticReseter>(AssemdefsForStaticReseter.FileName);
            var assemblies = assemdefCollection.assemblies;
            var assembliesSet = assemdefCollection.assembliesSet;

            if (assemdefCollection == null)
            {
                assemdefCollection = ScriptableObject.CreateInstance<AssemdefsForStaticReseter>();
                string path = $"Assets/Resources/{AssemdefsForStaticReseter.FileName}.asset";
                AssetDatabase.CreateAsset(assemdefCollection, path);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                throw new Exception(
                    $"[{nameof(StaticMemberReseter)}]" +
                    $" Cannot reset static members." +
                    $" You should add assemdefs to {AssemdefsForStaticReseter.FileName} by {path}");
            }

            if (types.Count == 0)
            {
                var sw = new Stopwatch();
                sw.Start();
                
                for (int i = 0; i < assemblies.Count; i++)
                {
                    var assem = assemblies[i];
                    RememberFieldsValues(assem, assembliesSet);
                }
                
                sw.Stop();
                Debug.Log(sw.ElapsedMilliseconds);
            }
            else
            {
                var sw2 = new Stopwatch();
                sw2.Start();
                ResetAllStaticsVariables();
                sw2.Stop();
                Debug.Log(sw2.ElapsedMilliseconds);
            }
        }

        private static void RememberFieldsValues(AssemblyDefinitionAsset assem, HashSet<string> set)
        {
            var allTypes = Assembly.Load(assem.name).GetTypes();
            var typesToAdd = new List<Type>();

            for (int i = 0; i < allTypes.Length; i++)
            {
                var type = allTypes[i];

                if (type.IsEnum)
                {
                    continue;
                }

                do
                {
                    if (checkedTypes.Contains(type))
                    {
                        break;
                    }

                    checkedTypes.Add(type);

                    if (type.ContainsGenericParameters || types.ContainsKey(type))
                    {
                        type = type.BaseType;
                        continue;
                    }

                    AddType(type, GetAllStaticFields(type));
                    typesToAdd.Add(type);
                    
                    type = type.BaseType;
                } 
                while (type != null && set.Contains(type.Assembly.GetName().Name));
            }
        }

        private static void ResetAllStaticsVariables()
        {
            foreach (var type in types.Keys)
            {
                var targetFields = types[type];

                for (int i = 0; i < targetFields.Count; i++)
                {
                    var data = targetFields[i];

                    object value = null;
                    var canSet = true;
                
                    if (data.stream != null)
                    {
                        data.stream.Position = 0;

                        if (data.stream.Length > 0)
                        {
                            try
                            {
                                value = BinaryDeserialize(data.stream);
                            }
                            catch (Exception e)
                            {
                                canSet = false;
                                Debug.LogError($"[{nameof(StaticMemberReseter)}] {e}");
                            }
                        }
                    }
                    else
                    {
                        try
                        {
                            value = JsonConvert.DeserializeObject(data.json, data.fieldInfo.FieldType, settings);
                        }
                        catch (Exception e)
                        {
                            canSet = false;
                            Debug.LogError($"[{nameof(StaticMemberReseter)}] {e}");
                        }
                    }

                    if (canSet)
                    {
                        try
                        {
                            data.fieldInfo.SetValue(null, value);
                        }
                        catch (Exception e)
                        {
                            Debug.LogError($"[{nameof(StaticMemberReseter)}] {data.fieldInfo.Name} {e}");
                        }
                    }
                }
            }
        }

        private static List<FieldInfo> GetAllStaticFields(Type type)
        {
            const BindingFlags flags =
                BindingFlags.Static
                | BindingFlags.NonPublic
                | BindingFlags.Public;
            
            var fields = type.GetFields(flags);
            var list = new List<FieldInfo>();
            
            for (int i = 0; i < fields.Length; i++)
            {
                var field = fields[i];

                if (field.IsLiteral && !field.IsInitOnly)
                {
                    continue;
                }
                
                list.Add(field);
            }

            return list;
        }
        
        private static FieldInfo[] GetConstants(System.Type type)
        {
            ArrayList constants = new ArrayList();

            FieldInfo[] fieldInfos = type.GetFields(
                // Gets all public and static fields

                BindingFlags.Public | BindingFlags.Static | 
                // This tells it to get the fields from all base types as well

                BindingFlags.FlattenHierarchy);

            // Go through the list and only pick out the constants
            foreach(FieldInfo fi in fieldInfos)
                // IsLiteral determines if its value is written at 
                //   compile time and not changeable
                // IsInitOnly determines if the field can be set 
                //   in the body of the constructor
                // for C# a field which is readonly keyword would have both true 
                //   but a const field would have only IsLiteral equal to true
                if(fi.IsLiteral && !fi.IsInitOnly)
                    constants.Add(fi);           

            // Return an array of FieldInfos
            return (FieldInfo[])constants.ToArray(typeof(FieldInfo));
        }

        private static void AddType(Type type, List<FieldInfo> fields)
        {
            if (fields.Count == 0)
            {
                return;
            }

            var list = new List<MemberData>();
            types.Add(type, list);

            for (int i = 0; i < fields.Count; i++)
            {
                var fieldInfo = fields[i];
                object fieldValue = null;
                fieldValue = fieldInfo.GetValue(null);

                if (typeof(Delegate).IsAssignableFrom(fieldInfo.FieldType))
                {
                    BinarySerialize(fieldValue, list, fieldInfo);
                }
                else
                {
                    JsonSerialize(fieldValue, list, fieldInfo);
                }
            }
        }

        private static void JsonSerialize(object fieldValue, List<MemberData> list, FieldInfo fieldInfo)
        {
            var json = string.Empty;

            if (fieldValue != null)
            {
                json = JsonConvert.SerializeObject(fieldValue, settings);
                
            }

            var data = new MemberData
            {
                json = json,
                fieldInfo = fieldInfo
            };
                        
            list.Add(data);
        }

        private static void SharpSerialize(object fieldValue, List<MemberData> list, FieldInfo fieldInfo)
        {
            var stream = new MemoryStream();
            var serializer = new SharpSerializer(true);

            if (fieldValue != null)
            {
                serializer.Serialize(fieldValue, stream);
            }
            
            var data = new MemberData
            {
                stream = stream,
                fieldInfo = fieldInfo
            };
            
            list.Add(data);
        }
        
        private static object SharpDeserialize(MemoryStream stream)
        {
            var serializer = new SharpSerializer(true);
            return serializer.Deserialize(stream);
        }

        private static void BinarySerialize(object fieldValue, List<MemberData> list, FieldInfo fieldInfo)
        {
            var stream = new MemoryStream();
            var serializer = new BinaryFormatter();

            if (fieldValue != null)
            {
                serializer.Serialize(stream, fieldValue);
            }
            
            var data = new MemberData
            {
                stream = stream,
                fieldInfo = fieldInfo
            };
            
            list.Add(data);
        }
        
        private static object BinaryDeserialize(MemoryStream stream)
        {
            var serializer = new BinaryFormatter();
            return serializer.Deserialize(stream);
        }
    }
}