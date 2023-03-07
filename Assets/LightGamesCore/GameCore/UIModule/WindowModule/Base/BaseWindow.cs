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

    protected virtual Transform Parent => null;
    public RectTransform RectTransform { get; private set; }
    protected virtual bool ShowByDefault => false;
    public virtual int SortingOrder => 0;

    protected override void Init()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0;
        GetComponent<Canvas>().sortingOrder = SortingOrder;

        gameObject.SetActive(false);
        transform.SetParent(Parent);
        RectTransform = (RectTransform)transform;
        if (ShowByDefault) { Show(); }
        else { Hide(); }
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
        showTween = GetShowAnimation().OnComplete(new TweenCallback(onComplete));
    }
    
    protected virtual void AnimateOnHiding(Action onComplete)
    {
        showTween?.Kill();
        hideTween = GetHideAnimation().OnComplete(new TweenCallback(onComplete));
    }

    protected virtual Tween GetShowAnimation()
    {
        return canvasGroup.DOFade(1, fadeSpeed);
    }
    
    protected virtual Tween GetHideAnimation()
    {
        return canvasGroup.DOFade(0, fadeSpeed);
    }

    public static void Show() => Instance.InternalShow();

    public static void Hide() => Instance.InternalHide();
}