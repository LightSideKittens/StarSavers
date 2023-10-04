using BeatRoyale.Interfaces;
using BeatRoyale.Windows;
using GameCore.Attributes;
using GameCore.Battle.Data;
using LSCore;
using UnityEngine;

namespace BeatRoyale.Launcher
{
    public class LauncherWorld : ServiceManager
    {
        [ColoredField, SerializeField] private CastleBackAnimator animator;
        [SerializeField] private Cards cards;

        protected override void Awake()
        {
            base.Awake();
            BaseInitializer.Initialize(Init);
        }

        private void Init()
        {
            animator.Init();
            cards.Init();
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

