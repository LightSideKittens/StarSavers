#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityToolbarExtender;
using static Core.ConfigModule.BaseConfig<Core.ConfigModule.DebugData>;

namespace GameCore.Battle
{
    [InitializeOnLoad]
    public static partial class RadiusUtils
    {
        static partial void OnTool()
        {
            ToolbarExtender.LeftToolbarGUI.Add(OnToolbarLeftGUI);
        }
        
        static void OnToolbarLeftGUI()
        {
            var needShowRadius = Config.needShowRadius;
            
            if (needShowRadius != GUILayout.Toggle(needShowRadius, "Show Radius"))
            {
                Config.needShowRadius = !needShowRadius;
                Save();
                SetActiveRadiuses(!needShowRadius);
            }
        }
    }
}
#endif