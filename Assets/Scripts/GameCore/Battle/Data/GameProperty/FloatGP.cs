using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace Battle.Data.GameProperty
{
    [Serializable]
    public abstract class BaseGameProperty : GamePropertyDrawer
    {
        public abstract void Init(string entityName, int level);
    }

    [Serializable]
    public abstract class BaseGameProperty<T> : BaseGameProperty where T : BaseGameProperty<T>
    {
        protected abstract T This { get; }
        public static Dictionary<string, Dictionary<int, T>> AllProps { get; private set; } = new();
        
        public override void Init(string entityName, int level)
        {
            if (!AllProps.TryGetValue(entityName, out var dict))
            {
                dict = new Dictionary<int, T>();
                AllProps.Add(entityName, dict);
            }
            
            dict.Add(level, This);
        }

        public abstract void Upgrade(T property);
    }

    [Serializable]
    public abstract class FloatAndPercent : BaseGameProperty<FloatAndPercent>
    {
        protected override FloatAndPercent This => this;
        [CustomValueDrawer("DrawValue")] public float value;
        [CustomValueDrawer("DrawPercent")] public int percent;

        public override void Upgrade(FloatAndPercent property)
        {
            value += property.value;
            value *= percent / 100f;
        }

#if UNITY_EDITOR
        protected virtual float DrawValue(float val, GUIContent label, Func<GUIContent, bool> _)
        {
            return EditorGUILayout.FloatField(label, val);
        }
        
        protected virtual int DrawPercent(int val, GUIContent label, Func<GUIContent, bool> _)
        {
            return EditorGUILayout.IntSlider(label, val, 0, 100);
        }
#endif
    }
}
