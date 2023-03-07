using GameCore.Attributes;
using GameCore.Common.SingleServices.Windows;
using UnityEngine;

namespace Common.SingleServices.Windows
{
    public class AnimatableWindow : BaseWindow<AnimatableWindow>
    {
        [SerializeField] private Transform spawnPoint;
        [ColoredField, SerializeField] private AnimText animText;
        [ColoredField, SerializeField] private HealthBar healthBar;
        public static Transform SpawnPoint => Instance.spawnPoint;
        internal static AnimText AnimText => Instance.animText;
        internal static HealthBar HealthBar => Instance.healthBar;
        protected override bool ShowByDefault => true;
        public override int SortingOrder => 5;

        protected override void Init()
        {
            base.Init();
            DontDestroyOnLoad(this);
        }
    }
}