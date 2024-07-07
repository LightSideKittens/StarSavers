using UnityEngine;
using static MultiWars.DebugData;

namespace MultiWars
{
    public static partial class ToolBar
    {
        private static void OnToolbarLeftGUI()
        {
            var needShowRadius = Config.needShowRadius;
            var serverEnabled = Config.serverEnabled;

            if (DrawToggle("Server Enabled", "Server Disabled", serverEnabled))
            {
                Config.serverEnabled = !serverEnabled;
                Savee();
            }

            if (DrawToggle("Radius Showed", "Radius Hidden", needShowRadius))
            {
                Config.needShowRadius = !needShowRadius;
                Savee();
            }
        }

        private static bool DrawToggle(string enabledLabel, string disabledLabel, bool isEnabled, int width = 100)
        {
            return GUILayout.Button(isEnabled ? $"<b>{enabledLabel}</b>" : $"<b>{disabledLabel}</b>",
                isEnabled ? GreenButtonStyle : RedButtonStyle,
                GUILayout.MaxWidth(width), GUILayout.MinHeight(20));
        }
    }
}