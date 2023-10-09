using LSCore.Extensions.Unity;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;
using RectExtensions = Sirenix.Utilities.RectExtensions;

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
            SirenixGUIStyles.BoxHeaderStyle.fixedHeight = 22;
            SirenixEditorGUI.BeginBoxHeader();
            isExpanded.Value = SirenixEditorGUI.Foldout(isExpanded.Value, label);
            
            if (SirenixEditorGUI.IconButton(EditorIcons.Plus))
            {
                this.ValueEntry.SmartValue.CreateData();
            }
            
            SirenixEditorGUI.EndBoxHeader();
            
            if (SirenixEditorGUI.BeginFadeGroup(this, isExpanded.Value))
            {
                EditorGUI.indentLevel = 1;
                
                for (int i = 0; i < Property.Children.Count; i++)
                {
                    var child = Property.Children[i];
                    EditorGUILayout.BeginHorizontal();
                    var widths = new float[] { 50, 150 };
                    
                    for (int j = 0; j < child.Children.Count; j++)
                    {
                        var childChild = child.Children[j];
                        var childLabel = childChild.Label;
                        float labelWidth = GUI.skin.label.CalcSize(childLabel).x + 20;
                        float oldLabelWidth = EditorGUIUtility.labelWidth;
                        EditorGUIUtility.labelWidth = labelWidth;
                        EditorGUIUtility.fieldWidth = widths[j];
                        childChild.Draw();
                        EditorGUIUtility.labelWidth = oldLabelWidth;
                    }

                    if (SirenixEditorGUI.IconButton(EditorIcons.Minus))
                    {
                        ValueEntry.SmartValue.RemoveAt(i);
                    }
                    
                    EditorGUILayout.EndHorizontal();
                }
                
                EditorGUI.indentLevel = 0;
            }
            
            SirenixEditorGUI.EndFadeGroup();
            SirenixEditorGUI.EndBox();
        }
    }
}