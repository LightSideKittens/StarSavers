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
            SirenixEditorGUI.BeginBox();
            {
                SirenixGUIStyles.BoxHeaderStyle.fixedHeight = 22;
                SirenixEditorGUI.BeginBoxHeader();
                {
                    isExpanded.Value = SirenixEditorGUI.Foldout(isExpanded.Value, label);

                    if (SirenixEditorGUI.IconButton(EditorIcons.Plus))
                    {
                        ValueEntry.SmartValue.CreateData();
                    }
                }
                SirenixEditorGUI.EndBoxHeader();

                float oldLabelWidth = EditorGUIUtility.labelWidth;
                EditorGUIUtility.labelWidth = 0;
                
                if (SirenixEditorGUI.BeginFadeGroup(this, isExpanded.Value))
                {
                    EditorGUI.indentLevel = 1;
                    var rect = EditorGUILayout.GetControlRect();
                    
                    for (int i = 0; i < Property.Children.Count; i++)
                    {
                        var child = Property.Children[i];
                        EditorGUILayout.BeginHorizontal();
                        var widths = new float[] { 1, 5 };

                        for (int j = 0; j < child.Children.Count; j++)
                        {
                            var childChild = child.Children[j];
                            EditorGUIUtility.fieldWidth = widths[j];
                            childChild.Draw(GUIContent.none);
                        }

                        if (SirenixEditorGUI.IconButton(EditorIcons.Minus))
                        {
                            ValueEntry.SmartValue.RemoveAt(i);
                        }

                        EditorGUILayout.EndHorizontal();
                        EditorGUILayout.Space(2);
                    }

                    EditorGUI.indentLevel = 0;
                }
                SirenixEditorGUI.EndFadeGroup();
                
                EditorGUIUtility.labelWidth = oldLabelWidth;
            }
            EditorGUILayout.Space(2);
            SirenixEditorGUI.EndBox();
        }
    }
}