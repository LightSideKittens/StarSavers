using Battle.Windows;
using LGCore.ConfigModule;
using LGCore.ConfigModule.Editor;
using UnityEngine;
using UnityToolbarExtender;
using static LGCore.ConfigModule.BaseConfig<LGCore.ConfigModule.DebugData>;
namespace BeatRoyale
{
    public static partial class ToolBar
    {
        private static void OnToolbarLeftGUI()
        {
            ConfigureToggleButtonStyle();
            var infinityMana = Config.infinityMana;
            var needShowRadius = Config.needShowRadius;
            var serverEnabled = Config.serverEnabled;
            GUI.changed = ToolbarExtender.leftRect.Contains(Event.current.mousePosition);

            if (DrawToggle("Server Enabled", "Server Disabled", serverEnabled))
            {
                Config.serverEnabled = !serverEnabled;
                ConfigUtils.Save<DebugData>();
            }

            if (DrawToggle("Radius Showed", "Radius Hidden", needShowRadius))
            {
                Config.needShowRadius = !needShowRadius;
                ConfigUtils.Save<DebugData>();
            }

            if (DrawToggle("Infinity Mana Enabled", "Infinity Mana Disabled", infinityMana, 150))
            {
                Config.infinityMana = !infinityMana;
                DeckWindow.InfinityMana = !infinityMana;
                ConfigUtils.Save<DebugData>();
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