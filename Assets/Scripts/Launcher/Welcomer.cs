using DG.Tweening;
using LSCore;
using LSCore.AnimationsModule.Animations;
using LSCore.Async;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace MultiWars
{
    public class Welcomer : ServiceManager<Welcomer>
    {
        [SerializeField] private SliderAnim sliderAnim;
        private Tween tween;
        
        protected override void Awake()
        {
            base.Awake();
            tween = sliderAnim.Animate();
            Addressables.LoadSceneAsync("Launcher").OnSuccess(OnSuccess).OnError(OnError);
            
            return;

            void OnSuccess()
            {
                tween.Kill();
            }

            void OnError()
            {
                tween.Kill();
            }
        }
    }
}