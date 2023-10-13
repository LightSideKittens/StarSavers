using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

namespace Battle.Data.GameProperty
{
    public class PropConverter : JsonConverter<Prop>
    {
        public override Prop ReadJson(JsonReader reader, Type objectType, Prop existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (hasExistingValue)
            {
                return existingValue;
            }
            
            existingValue.Init((string)reader.Value);
            return existingValue;
        }

        public override void WriteJson(JsonWriter writer, Prop value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, (string)value);
        }
    }
    
    [Serializable]
    [JsonConverter(typeof(PropConverter))]
    public struct Prop : ISerializationCallbackReceiver, IEnumerable<KeyValuePair<string, float>>
    {
        private Dictionary<string, float> value;
        [SerializeField] private string serializedValue;
        
        public Dictionary<string, float> Value 
        {
            get
            {
                value ??= new Dictionary<string, float>();
                return value;
            }
        }

        public void OnBeforeSerialize() => serializedValue = this;
        public void OnAfterDeserialize() => Init(serializedValue);

        
        private static readonly StringBuilder stringBuilder = new();
        public const string FloatRegex = @"(\w+):(-?\d+(\.\d+)?)";
        
        public static implicit operator string(Prop a)
        {
            stringBuilder.Clear();
            
            foreach (var (label, value) in a.Value)
            {
                stringBuilder.Append($"{label}:{value} ");
            }

            return stringBuilder.ToString();
        }
        
        public static Prop Copy(Prop prop)
        {
            var result = new Prop
            {
                value = new Dictionary<string, float>(prop.value)
            };
            
            return result;
        }
        
        public static Prop Create(string props)
        {
            return string.IsNullOrEmpty(props) ? default : new Prop().Init(props);
        }

        public Prop Init(string props)
        {
            var matches = Regex.Matches(props, FloatRegex);
            foreach (Match match in matches)
            {
                var val = float.Parse(match.Groups[2].Value);
                Value[match.Groups[1].Value] = val;
            }

            return this;
        }

#if UNITY_EDITOR
        public string SerializedValue => this;
        
        public void DrawFloat(string name)
        {
            Value.TryGetValue(name, out var floatValue);
            Value[name] = EditorGUILayout.FloatField(name, floatValue);
        }
        
        public void DrawSlider(string name, float min, float max)
        {
            Value.TryGetValue(name, out var floatValue);
            Value[name] = EditorGUILayout.Slider(name, floatValue, min, max);
        }
#endif

        public IEnumerator<KeyValuePair<string, float>> GetEnumerator()
        {
            return Value.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}