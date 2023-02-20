using System;
using Core.SingleService;
using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(Canvas))]
[RequireComponent(typeof(CanvasGroup))]
public abstract class BaseWindow<T> : SingleService<T> where T : BaseWindow<T>
{
    public static event Action<BaseWindow<T>> ShowingWindow;
    public static event Action<BaseWindow<T>> HidingWindow;
    public static event Action Showing;
    public static event Action Hiding;
    
    public static event Action<BaseWindow<T>> ShowedWindow;
    public static event Action<BaseWindow<T>> HiddenWindow;
    public static event Action Showed;
    public static event Action Hidden;

    [SerializeField] protected float fadeSpeed = 0.2f;
    private CanvasGroup canvasGroup;
    private Tween showTween;
    private Tween hideTween;

    protected override void Init()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0;

        gameObject.SetActive(false);
    }

    private void InternalShow()
    {
        ShowingWindow?.Invoke(this);
        Showing?.Invoke();
        
        OnShowing();
        AnimateOnShowing(OnCompleteShow);
    }
    
    private void InternalHide()
    {
        HidingWindow?.Invoke(this);
        Hiding?.Invoke();
        
        OnHiding();
        AnimateOnHiding(OnCompleteHide);
    }
    
    private void OnCompleteShow()
    {
        OnShowed();
        
        ShowedWindow?.Invoke(this);
        Showed?.Invoke();
    }

    private void OnCompleteHide()
    {
        OnHidden();
        
        HiddenWindow?.Invoke(this);
        Hidden?.Invoke();
    }

    protected virtual void OnShowing()
    {
        gameObject.SetActive(true);
    }
    
    protected virtual void OnHiding() { }

    protected virtual void OnShowed() { }

    protected virtual void OnHidden()
    {
        gameObject.SetActive(false);
    }

    protected virtual void AnimateOnShowing(Action onComplete)
    {
        hideTween?.Kill();
        showTween = canvasGroup.DOFade(1, fadeSpeed).OnComplete(new TweenCallback(onComplete));
    }
    
    protected virtual void AnimateOnHiding(Action onComplete)
    {
        showTween?.Kill();
        hideTween = canvasGroup.DOFade(0, fadeSpeed).OnComplete(new TweenCallback(onComplete));
    }

    public static void Show() => Instance.InternalShow();

    public static void Hide() => Instance.InternalHide();
}