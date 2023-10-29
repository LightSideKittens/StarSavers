using Attributes;
using LSCore;
using UnityEngine;

namespace Animatable
{
    public class AnimatableWindow : BaseWindow<AnimatableWindow>
    {
        [ColoredField, SerializeField] private AnimText animText;
        [ColoredField, SerializeField] private HealthBar healthBar;
        [ColoredField, SerializeField] private HealthBar opponentHealthBar;
        [ColoredField, SerializeField] private Loader loader;
        
        public static Camera Cam { get; private set; }
        public static Transform SpawnPoint => Instance.transform;
        internal static AnimText AnimText => Instance.animText;
        internal static HealthBar HealthBar => Instance.healthBar;
        internal static HealthBar OpponentHealthBar => Instance.opponentHealthBar;
        internal static Loader Loader => Instance.loader;
        protected override bool ShowByDefault => true;
        public override int SortingOrder => 5;

        protected override void Init()
        {
            base.Init();
            DontDestroyOnLoad(this);
            Cam = Camera.main;
            animText.Init();
        }

        internal static Vector3 GetLocalPosition(Vector3 worldPos)
        {
            var targetLocalPosByCam = Cam.transform.InverseTransformPoint(worldPos);
            targetLocalPosByCam /= Canvas.transform.lossyScale.x;
            targetLocalPosByCam.z = 0;
            return targetLocalPosByCam;
        }
        
        public static void Clean()
        {
            var instance = Instance;
            for (int i = 1; i < instance.transform.childCount; i++)
            {
                Destroy(instance.transform.GetChild(i).gameObject);
            }
        }
    }
}