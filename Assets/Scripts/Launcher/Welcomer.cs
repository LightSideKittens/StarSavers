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
            Wait.Delay(1, () =>
            {
                SingleAsset<ExchangeTable>.Get("ExchangeTable");
                SingleAsset<Sprite>.Get("back-01");
                Palette.TryGet("Red", out var color);
                Debug.Log(color);
            });
            
            Wait.Delay(2, () =>
            {
                SceneManager.LoadSceneAsync("Launcher");
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