
using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class PopupController: MonoBehaviour, IGameUI
{
    [SerializeField] protected RectTransform panelRectTransform;      // 팝업창
    protected CanvasGroup _backgroundCanvasGroup;                     // 뒤에 시커먼 배경

    public delegate void PanelControllerHideDelegate();
    
    protected virtual void Awake()
    {
        _backgroundCanvasGroup = GetComponent<CanvasGroup>();
    }

    public UniTask Show()
    {
        _backgroundCanvasGroup.alpha = 0;
        panelRectTransform.localScale = Vector3.zero;
        gameObject.SetActive(true);
        
        var tcs = new UniTaskCompletionSource();
        
        _backgroundCanvasGroup.DOFade(1, 0.3f).SetEase(Ease.Linear).SetUpdate(true);
        panelRectTransform.DOScale(1, 0.3f).SetEase(Ease.OutBack).SetUpdate(true).OnComplete(() =>
        {
            tcs.TrySetResult();
        });
        
        return tcs.Task;
    }

    public virtual UniTask Hide()
    {
        _backgroundCanvasGroup.alpha = 1;
        panelRectTransform.localScale = Vector3.one;
        
        var tcs = new UniTaskCompletionSource();
    
        var sequence = DOTween.Sequence().SetUpdate(true);
        sequence.Join(_backgroundCanvasGroup.DOFade(0, 0.3f).SetEase(Ease.Linear))
            .Join(panelRectTransform.DOScale(0, 0.3f).SetEase(Ease.InBack))
            .OnComplete(() =>
            {
                gameObject.SetActive(false);
                tcs.TrySetResult();
            });

        return tcs.Task;
    }
    
    // public async void Hide(PanelControllerHideDelegate hideDelegate)
    // {
    //     await HideAsync();
    //     hideDelegate?.Invoke();
    // }
    
    protected async UniTask HideAsync()
    {
        _backgroundCanvasGroup.alpha = 1;
        panelRectTransform.localScale = Vector3.one;
    
        var sequence = DOTween.Sequence().SetUpdate(true);
        sequence.Join(_backgroundCanvasGroup.DOFade(0, 0.3f).SetEase(Ease.Linear))
            .Join(panelRectTransform.DOScale(0, 0.3f).SetEase(Ease.InBack))
            .OnComplete(() =>
            {
                gameObject.SetActive(false);
            });

        await sequence.AsyncWaitForCompletion();
    }
}
