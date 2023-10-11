using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace Battle.Data.GameProperty
{
    public static class StringExtensions
    {
        public static float GetFloat(this string target)
        {
            if (string.IsNullOrEmpty(target)) return -1;
            
            var match = Regex.Match(target, BaseGameProperty.FloatRegex);
            return float.Parse(match.Groups[2].Value);
        }
        
        public static float GetFloat(this string target, int index)
        {
            if (string.IsNullOrEmpty(target)) return -1;
            
            var matches = Regex.Matches(target, BaseGameProperty.FloatRegex);
            return float.Parse(matches[index].Groups[2].Value);
        }
        
        public static IEnumerable<float> GetFloats(this string target, int index)
        {
            if (string.IsNullOrEmpty(target)) yield break;
            
            var matches = Regex.Matches(target, BaseGameProperty.FloatRegex);

            for (int i = 0; i < matches.Count; i++)
            {
                yield return float.Parse(matches[i].Groups[2].Value);
            }
        }
    }
    
    [Serializable]
    public abstract class BaseGameProperty : GamePropertyDrawer
    {
        public const string FloatRegex = @"(\w+):(-?\d+(\.\d+)?)";

        [CustomValueDrawer("DrawValue")] [SerializeField] private string value;
        public abstract PropBuilder DefaultValue { get; }
        public string Name => GetType().Name;
        public BaseGameProperty()
        {
            value = DefaultValue;
        }

        public void TryInit()
        {
            if (result == null)
            {
                result = new Dictionary<string, float>();
                Deserialize(value, result);
            }
        }
        
        public abstract string Upgrade(string propValue);

        public static void Deserialize(string target, Dictionary<string, float> floatFields)
        {
            if (string.IsNullOrEmpty(target)) return;

            floatFields.Clear();
            var matches = Regex.Matches(target, FloatRegex);
            foreach (Match match in matches)
            {
                var val = float.Parse(match.Groups[2].Value);
                floatFields.Add(match.Groups[1].Value, val);
            }
        }

        private static StringBuilder builder = new();
        [NonSerialized] public Dictionary<string, float> result;

        public string Serialize()
        {
            TryInit();
            return Serialize(result);
        }

        public static string Serialize(Dictionary<string, float> floatFields)
        {
            builder.Clear();

            foreach (var (label, value) in floatFields)
            {
                builder.Append($"{label}:{value} ");
            }

            return builder.ToString();
        }
        
#if UNITY_EDITOR
        public static List<Type> AllPropertyTypes { get; private set; }

        static BaseGameProperty()
        {
            var type = typeof(BaseGameProperty);
            var assembly = Assembly.GetAssembly(type);
            var types = assembly.GetTypes();
            AllPropertyTypes = new List<Type>();

            for (int i = 0; i < types.Length; i++)
            {
                var t = types[i];

                if (t.IsSubclassOf(type) && !t.IsAbstract && !t.IsInterface)
                {
                    AllPropertyTypes.Add(t);
                }
            }
        }

        protected Dictionary<string, float> floatFields;

        private string DrawValue(string val, GUIContent _, Func<GUIContent, bool> __)
        {
            result ??= new Dictionary<string, float>();
            floatFields ??= new Dictionary<string, float>();
            Deserialize(val, floatFields);
            result.Clear();
            DrawFields();

            return Serialize(result);
        }

        public virtual void DrawFields()
        {
            foreach (var (label, floatValue) in floatFields)
            {
                result[label] = EditorGUILayout.FloatField(label, floatValue);
            }
        }
#endif

    }

    [Serializable]
    public abstract class FloatAndPercent : BaseGameProperty
    {
        public const string ValueKey = "Value";
        public const string PercentKey = "Percent";
        
        public override PropBuilder DefaultValue => PropBuilder.Create().Float(ValueKey, 0).Float(PercentKey, 0);

        public override string Upgrade(string propValue)
        {
            TryInit();
            var propertyResult = new Dictionary<string, float>();
            Deserialize(propValue, propertyResult);
            propertyResult[ValueKey] += result[ValueKey];
            propertyResult[ValueKey] *= 1 + result[PercentKey] / 100;
            return Serialize(propertyResult);
        }

#if UNITY_EDITOR
        public override void DrawFields()
        {
            result[ValueKey] = EditorGUILayout.FloatField(ValueKey, floatFields[ValueKey]);
            result[PercentKey] = EditorGUILayout.Slider(PercentKey, floatFields[PercentKey], 0, 100);
        }
#endif
    }
}
