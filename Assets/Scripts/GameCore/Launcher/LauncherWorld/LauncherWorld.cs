using BeatRoyale.Interfaces;
using BeatRoyale.Windows;
using GameCore.Battle.Data;
using LSCore;
using UnityEngine;

namespace BeatRoyale.Launcher
{
    public class LauncherWorld : ServiceManager
    {
        [SerializeField] private Cards cards;

        protected override void Awake()
        {
            base.Awake();
            BaseInitializer.Initialize(Init);
        }

        private void Init()
        {
            cards.Init();
            ControlPanel.Show();
        }
    }
}

