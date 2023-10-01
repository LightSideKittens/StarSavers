using UnityEngine;
using UnityToolbarExtender;
using static BeatRoyale.DebugData;

namespace BeatRoyale
{
    public static partial class ToolBar
    {
        private static void OnToolbarLeftGUI()
        {
            ConfigureToggleButtonStyle();
            var d = Config.infinityMana;
            var infinityMana = Config.infinityMana;
            var needShowRadius = Config.needShowRadius;
            var serverEnabled = Config.serverEnabled;
            GUI.changed = ToolbarExtender.leftRect.Contains(Event.current.mousePosition);

            if (DrawToggle("Server Enabled", "Server Disabled", serverEnabled))
            {
                Config.serverEnabled = !serverEnabled;
                Save();
            }

            if (DrawToggle("Radius Showed", "Radius Hidden", needShowRadius))
            {
                Config.needShowRadius = !needShowRadius;
                Save();
            }

            if (DrawToggle("Infinity Mana Enabled", "Infinity Mana Disabled", infinityMana, 150))
            {
                Config.infinityMana = !infinityMana;
                Save();
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