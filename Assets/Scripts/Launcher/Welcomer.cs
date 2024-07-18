using DG.Tweening;
using LSCore;
using LSCore.AnimationsModule.Animations;
using LSCore.Async;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;

namespace MultiWars
{
    public class Welcomer : ServiceManager<Welcomer>
    {
        [SerializeField] private SliderAnim sliderAnim;
        [SerializeField] private SliderAnim completedSliderAnim;
        private Tween tween;
        
        protected override void Awake()
        {
            base.Awake();
            tween = sliderAnim.Animate();
            Wait.Delay(5, () =>
            {
                SceneManager.LoadScene("Launcher");
            });
            
            return;

            void OnSuccess()
            {
                tween.Kill();
                completedSliderAnim.Animate();
            }

            void OnError()
            {
                tween.Kill();
            }
        }
    }
}