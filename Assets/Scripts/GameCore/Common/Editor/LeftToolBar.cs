using Battle.Windows;
using Core.ConfigModule;
using UnityEditor;
using UnityEngine;
using UnityToolbarExtender;

namespace BeatRoyale
{
    [InitializeOnLoad]
    public class LeftToolBar
    {
        static LeftToolBar()
        {
            ToolbarExtender.LeftToolbarGUI.Add(OnToolbarLeftGUI);
        }

        private static void OnToolbarLeftGUI()
        {
            DebugData.Config.infinityMana = GUILayout.Toggle(DebugData.Config.infinityMana, "Infinity Mana", GUILayout.MaxWidth(100));

            if (Application.isPlaying && DeckWindow.IsInited)
            {
                DeckWindow.InfinityMana = DebugData.Config.infinityMana;
            }
        }
    }
}