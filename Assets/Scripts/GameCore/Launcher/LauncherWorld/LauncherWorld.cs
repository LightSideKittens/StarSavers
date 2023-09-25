using BeatRoyale.Windows;
using GameCore.Attributes;
using LGCore;
using UnityEngine;
using Initializer = BeatRoyale.Interfaces.BaseInitializer<BeatRoyale.Interfaces.IInitializer>;

namespace BeatRoyale.Launcher
{
    public class LauncherWorld : ServiceManager
    {
        [ColoredField, SerializeField] private CastleBackAnimator animator;

        protected override void Awake()
        {
            base.Awake();
            Initializer.Initialize(Init);
        }

        private void Init()
        {
            animator.Init();
            ControlPanel.Show();
        }
        
        private void Update()
        {
            animator.Update();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            animator.Deinit();
        }
    }
}

