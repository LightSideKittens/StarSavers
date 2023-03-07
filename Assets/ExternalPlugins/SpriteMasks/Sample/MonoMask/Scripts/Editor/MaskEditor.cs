#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace FastSpriteMask
{
    [CustomEditor(typeof(Mask))]
    [CanEditMultipleObjects]
    public class MaskEditor : Editor
    {
        SerializedProperty Script, MaskSprite, MaskType, AlphaCutoff;
        
        void OnEnable()
        {
            Script = serializedObject.FindProperty("m_Script");
            
            MaskSprite = serializedObject.FindProperty("maskSprite");
            MaskType = serializedObject.FindProperty("maskType");
            AlphaCutoff = serializedObject.FindProperty("alphaCutoff");
        }

        public override void OnInspectorGUI()
        { 
            serializedObject.Update();
            
            GUI.enabled = false;
            EditorGUILayout.PropertyField(Script, true, new GUILayoutOption[0]);
            GUI.enabled = true;
            
            EditorGUILayout.PropertyField(MaskSprite);
            EditorGUILayout.PropertyField(MaskType);
            var value = MaskType.intValue;
            if(value < 4)
                EditorGUILayout.PropertyField(AlphaCutoff);
            
            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif
