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
            var infinityMana = GUILayout.Toggle(DebugData.Config.infinityMana, "Infinity Mana", GUILayout.MaxWidth(100));

            if (DebugData.Config.infinityMana != infinityMana)
            {
                DebugData.Config.infinityMana = infinityMana;
                DebugData.Save();
            }
            
            if (Application.isPlaying && DeckWindow.IsInited)
            {
                DeckWindow.InfinityMana = DebugData.Config.infinityMana;
            }
        }
    }
}