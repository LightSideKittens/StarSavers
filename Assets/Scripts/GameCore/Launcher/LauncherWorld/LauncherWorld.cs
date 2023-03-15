using Battle.Data;
using BeatRoyale.Windows;
using Core.SingleService;
using GameCore.Attributes;
using UnityEngine;

namespace BeatRoyale.Launcher
{
    public partial class LauncherWorld : ServiceManager
    {
        [ColoredField, SerializeField] private CastleBackAnimator animator;
        [SerializeField] private LevelsConfigsManager levelsConfigsManager;

        protected override void Awake()
        {
            base.Awake();
            Initialize(Init);
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

