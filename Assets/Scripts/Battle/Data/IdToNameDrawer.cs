#if UNITY_EDITOR
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace GameCore.Battle.Data
{
    public class IdToNameDrawer : OdinValueDrawer<IdToName>
    {
        private LocalPersistentContext<bool> isExpanded;
        
        protected override void Initialize()
        {
            isExpanded = this.GetPersistentValue<bool>(
                "ColoredFoldoutGroupAttributeDrawer.isExpanded",
                GeneralDrawerConfig.Instance.ExpandFoldoutByDefault);
        }
        
        protected override void DrawPropertyLayout(GUIContent label)
        {
            var value = (IdToName)Property.ValueEntry.WeakSmartValue;
            SirenixEditorGUI.BeginBox();
            {
                SirenixGUIStyles.BoxHeaderStyle.fixedHeight = 22;
                SirenixEditorGUI.BeginBoxHeader();
                {
                    isExpanded.Value = SirenixEditorGUI.Foldout(isExpanded.Value, label);

                    if (SirenixEditorGUI.IconButton(EditorIcons.Plus))
                    {
                        value.CreateData();
                        this.ForceSaveParent();
                    }
                }
                SirenixEditorGUI.EndBoxHeader();

                float oldLabelWidth = EditorGUIUtility.labelWidth;
                float oldFieldWidth = EditorGUIUtility.fieldWidth;
                EditorGUIUtility.labelWidth = 0;
                
                if (SirenixEditorGUI.BeginFadeGroup(this, isExpanded.Value))
                {
                    EditorGUI.indentLevel = 1;
                    var rect = EditorGUILayout.GetControlRect();
                    
                    for (int i = 0; i < Property.Children.Count; i++)
                    {
                        var child = Property.Children[i].Children;
                        EditorGUILayout.BeginHorizontal();

                        for (int j = 0; j < child.Count; j++)
                        {
                            child[j].Draw();
                        }

                        if (SirenixEditorGUI.IconButton(EditorIcons.Minus))
                        {
                            value.RemoveAt(i);
                            this.ForceSaveParent();
                        }

                        EditorGUILayout.EndHorizontal();
                        EditorGUILayout.Space(2);
                    }

                    EditorGUI.indentLevel = 0;
                }
                SirenixEditorGUI.EndFadeGroup();
                
                EditorGUIUtility.labelWidth = oldLabelWidth;
                EditorGUIUtility.fieldWidth = oldFieldWidth;
            }
            EditorGUILayout.Space(2);
            SirenixEditorGUI.EndBox();
        }
    }
}
#endif
